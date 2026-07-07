using System.Net;
using System.Security.Claims;
using Aeroverra.Authentication.Kick;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Aeroverra.Authentication.Tests;

public class KickTests
{
    [Fact]
    public void Defaults_Use_The_Kick_Endpoints()
    {
        var options = new KickAuthenticationOptions();

        Assert.Equal("https://id.kick.com/oauth/authorize", options.AuthorizationEndpoint);
        Assert.Equal("https://id.kick.com/oauth/token", options.TokenEndpoint);
        Assert.Equal("https://api.kick.com/public/v1/users", options.UserInformationEndpoint);
        Assert.Equal("/signin-kick", options.CallbackPath.Value);
        Assert.Equal("Kick", options.ClaimsIssuer);
        Assert.True(options.UsePkce);
        Assert.Contains("user:read", options.Scope);
    }

    [Fact]
    public async Task Challenge_Redirects_To_Kick_With_Pkce()
    {
        using var host = await OAuthFlow.CreateHostAsync(RegisterKick, KickAuthenticationDefaults.AuthenticationScheme);
        using var client = host.GetTestServer().CreateClient();

        using var response = await client.GetAsync("/challenge");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        var location = response.Headers.Location!;
        Assert.Equal("id.kick.com", location.Host);
        Assert.Equal("/oauth/authorize", location.AbsolutePath);

        var query = QueryHelpers.ParseQuery(location.Query);
        Assert.Equal("test-client-id", query["client_id"].ToString());
        Assert.Equal("code", query["response_type"].ToString());
        Assert.Equal("https://localhost/signin-kick", query["redirect_uri"].ToString());
        Assert.Equal("S256", query["code_challenge_method"].ToString());
        Assert.False(string.IsNullOrEmpty(query["code_challenge"].ToString()));
        Assert.False(string.IsNullOrEmpty(query["state"].ToString()));
        Assert.Contains("user:read", query["scope"].ToString());
    }

    [Theory]
    [InlineData(ClaimTypes.NameIdentifier, "123456")]
    [InlineData(ClaimTypes.Name, "Space Ranger")]
    [InlineData(ClaimTypes.Email, "spaceranger@example.com")]
    [InlineData(KickAuthenticationConstants.Claims.ProfileImageUrl, "https://files.kick.com/images/user/123456/profile_image.png")]
    public async Task Can_Sign_In_Using_Kick(string claimType, string claimValue)
    {
        var claims = await OAuthFlow.AuthenticateAsync(RegisterKick, KickAuthenticationDefaults.AuthenticationScheme);

        var claim = Assert.Single(claims, claim => claim.Type == claimType);
        Assert.Equal(claimValue, claim.Value);
    }

    private static void RegisterKick(AuthenticationBuilder builder) => builder.AddKick(options =>
    {
        options.ClientId = "test-client-id";
        options.ClientSecret = "test-client-secret";
        options.BackchannelHttpHandler = new StubBackchannel(KickEndpoints);
    });

    private static HttpResponseMessage? KickEndpoints(HttpRequestMessage request)
    {
        return request.RequestUri!.GetLeftPart(UriPartial.Path) switch
        {
            "https://id.kick.com/oauth/token" when request.Method == HttpMethod.Post => StubBackchannel.Json(
                """
                {
                  "access_token": "kick-access-token",
                  "token_type": "Bearer",
                  "expires_in": 3600,
                  "refresh_token": "kick-refresh-token",
                  "scope": "user:read"
                }
                """),
            "https://api.kick.com/public/v1/users" when request.Method == HttpMethod.Get => StubBackchannel.Json(
                """
                {
                  "data": [
                    {
                      "user_id": 123456,
                      "name": "Space Ranger",
                      "email": "spaceranger@example.com",
                      "profile_picture": "https://files.kick.com/images/user/123456/profile_image.png"
                    }
                  ],
                  "message": "OK"
                }
                """),
            _ => null,
        };
    }
}
