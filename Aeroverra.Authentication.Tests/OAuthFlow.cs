using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aeroverra.Authentication.Tests;

/// <summary>
/// A claim observed on the signed-in user, serialized back from the test server.
/// </summary>
public sealed record ClaimData(string Type, string Value);

/// <summary>
/// Drives a full in-memory OAuth authorization code flow (challenge, provider callback,
/// cookie sign-in) against a provider registered on a TestServer, with the provider's
/// remote endpoints stubbed out via <see cref="StubBackchannel"/>.
/// </summary>
internal static class OAuthFlow
{
    public static async Task<IHost> CreateHostAsync(Action<AuthenticationBuilder> registerProvider, string scheme)
    {
        var host = new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.ConfigureServices(services =>
                {
                    var authentication = services.AddAuthentication(options =>
                    {
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    });

                    authentication.AddCookie();
                    registerProvider(authentication);
                });
                webHost.Configure(app =>
                {
                    app.UseAuthentication();
                    app.Use(async (context, next) =>
                    {
                        if (context.Request.Path == "/challenge")
                        {
                            await context.ChallengeAsync(scheme, new AuthenticationProperties { RedirectUri = "/me" });
                        }
                        else if (context.Request.Path == "/me")
                        {
                            var claims = context.User.Claims.Select(claim => new ClaimData(claim.Type, claim.Value));
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(JsonSerializer.Serialize(claims));
                        }
                        else
                        {
                            await next();
                        }
                    });
                });
            })
            .Build();

        await host.StartAsync();
        host.GetTestServer().BaseAddress = new Uri("https://localhost");
        return host;
    }

    public static async Task<IReadOnlyList<ClaimData>> AuthenticateAsync(Action<AuthenticationBuilder> registerProvider, string scheme)
    {
        using var host = await CreateHostAsync(registerProvider, scheme);
        using var client = host.GetTestServer().CreateClient();

        // Step 1: the challenge redirects to the provider's authorization endpoint
        // and sets the correlation cookie.
        using var challengeResponse = await client.GetAsync("/challenge");
        Assert.Equal(HttpStatusCode.Redirect, challengeResponse.StatusCode);

        var authorizationRedirect = challengeResponse.Headers.Location!;
        var query = QueryHelpers.ParseQuery(authorizationRedirect.Query);
        var state = query["state"].ToString();
        var redirectUri = new Uri(query["redirect_uri"].ToString());

        // Step 2: the provider redirects back to the callback with an authorization code.
        // The handler exchanges it for tokens and fetches the user profile over the
        // stubbed backchannel, then signs the user into the cookie scheme.
        using var callbackRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"{redirectUri.PathAndQuery}?code=fake-authorization-code&state={Uri.EscapeDataString(state)}");
        AddCookies(callbackRequest, challengeResponse);

        using var callbackResponse = await client.SendAsync(callbackRequest);
        Assert.Equal(HttpStatusCode.Redirect, callbackResponse.StatusCode);
        Assert.Equal("/me", callbackResponse.Headers.Location!.OriginalString);

        // Step 3: the session cookie issued by the sign-in identifies the user.
        using var profileRequest = new HttpRequestMessage(HttpMethod.Get, "/me");
        AddCookies(profileRequest, callbackResponse);

        using var profileResponse = await client.SendAsync(profileRequest);
        Assert.Equal(HttpStatusCode.OK, profileResponse.StatusCode);

        var json = await profileResponse.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<ClaimData>>(json)!;
    }

    private static void AddCookies(HttpRequestMessage request, HttpResponseMessage from)
    {
        if (!from.Headers.TryGetValues("Set-Cookie", out var values))
        {
            return;
        }

        // Forward name=value pairs, skipping cookie deletions (empty values).
        var pairs = values.Select(value => value.Split(';')[0]).Where(pair => !pair.EndsWith('='));
        var header = string.Join("; ", pairs);

        if (header.Length > 0)
        {
            request.Headers.Add("Cookie", header);
        }
    }
}

/// <summary>
/// An <see cref="HttpMessageHandler"/> that serves canned responses for a provider's
/// token and user information endpoints.
/// </summary>
internal sealed class StubBackchannel(Func<HttpRequestMessage, HttpResponseMessage?> responder) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = responder(request) ?? new HttpResponseMessage(HttpStatusCode.NotFound);
        response.RequestMessage = request;
        return Task.FromResult(response);
    }

    public static HttpResponseMessage Json(string json) => new(HttpStatusCode.OK)
    {
        Content = new StringContent(json, Encoding.UTF8, "application/json"),
    };
}
