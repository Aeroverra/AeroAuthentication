using System.Net;
using System.Security.Claims;
using Aeroverra.Authentication.OAuth.Aero;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Aeroverra.Authentication.Tests;

public class AeroTests
{
    [Fact]
    public void Defaults_Use_The_Aero_Endpoints()
    {
        var options = new AeroAuthenticationOptions();

        Assert.Equal("https://api.aero.vi/oauth/authorize", options.AuthorizationEndpoint);
        Assert.Equal("https://api.aero.vi/oauth/token", options.TokenEndpoint);
        Assert.Equal("https://api.aero.vi/user/userinfo", options.UserInformationEndpoint);
        Assert.Equal("/signin-aero", options.CallbackPath.Value);
        Assert.Equal("Aero", options.ClaimsIssuer);
        Assert.True(options.UsePkce);
        Assert.Contains("offline_access", options.Scope);
        Assert.Contains("user.profile.read", options.Scope);
    }

    [Fact]
    public async Task Challenge_Redirects_To_Aero_With_Pkce()
    {
        using var host = await OAuthFlow.CreateHostAsync(RegisterAero, AeroAuthenticationDefaults.AuthenticationScheme);
        using var client = host.GetTestServer().CreateClient();

        using var response = await client.GetAsync("/challenge");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        var location = response.Headers.Location!;
        Assert.Equal("api.aero.vi", location.Host);
        Assert.Equal("/oauth/authorize", location.AbsolutePath);

        var query = QueryHelpers.ParseQuery(location.Query);
        Assert.Equal("test-client-id", query["client_id"].ToString());
        Assert.Equal("code", query["response_type"].ToString());
        Assert.Equal("https://localhost/signin-aero", query["redirect_uri"].ToString());
        Assert.Equal("S256", query["code_challenge_method"].ToString());
        Assert.False(string.IsNullOrEmpty(query["code_challenge"].ToString()));
        Assert.False(string.IsNullOrEmpty(query["state"].ToString()));
        Assert.Contains("offline_access", query["scope"].ToString());
        Assert.Contains("user.profile.read", query["scope"].ToString());
    }

    [Theory]
    [InlineData(ClaimTypes.NameIdentifier, "aero-user-id")]
    [InlineData(ClaimTypes.Name, "Space Ranger")]
    [InlineData(ClaimTypes.Email, "spaceranger@example.com")]
    public async Task Can_Sign_In_Using_Aero(string claimType, string claimValue)
    {
        var claims = await OAuthFlow.AuthenticateAsync(RegisterAero, AeroAuthenticationDefaults.AuthenticationScheme);

        var claim = Assert.Single(claims, claim => claim.Type == claimType);
        Assert.Equal(claimValue, claim.Value);
    }

    private static void RegisterAero(AuthenticationBuilder builder) => builder.AddAero(options =>
    {
        options.ClientId = "test-client-id";
        options.ClientSecret = "test-client-secret";
        options.BackchannelHttpHandler = new StubBackchannel(AeroEndpoints);
    });

    private static HttpResponseMessage? AeroEndpoints(HttpRequestMessage request)
    {
        return request.RequestUri!.GetLeftPart(UriPartial.Path) switch
        {
            "https://api.aero.vi/oauth/token" when request.Method == HttpMethod.Post => StubBackchannel.Json(
                """
                {
                  "access_token": "aero-access-token",
                  "token_type": "Bearer",
                  "expires_in": 3600,
                  "refresh_token": "aero-refresh-token",
                  "scope": "offline_access user.profile.read"
                }
                """),
            "https://api.aero.vi/user/userinfo" when request.Method == HttpMethod.Get => StubBackchannel.Json(
                """
                {
                  "id": "aero-user-id",
                  "userName": "Space Ranger",
                  "email": "spaceranger@example.com"
                }
                """),
            _ => null,
        };
    }
}
