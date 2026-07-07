using System.Net;
using Aeroverra.Authentication.OpenIddict.Client.Kick;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenIddict.Client;
using OpenIddict.Client.AspNetCore;

namespace Aeroverra.Authentication.Tests;

public class KickOpenIddictTests
{
    [Fact]
    public void Registration_Uses_The_Static_Kick_Configuration()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOpenIddict().AddClient(options =>
        {
            options.AllowAuthorizationCodeFlow();
            options.AddEphemeralEncryptionKey().AddEphemeralSigningKey();
            options.UseAspNetCore();
            options.UseSystemNetHttp();

            options.AddKick(settings =>
            {
                settings.ClientId = "test-client-id";
                settings.ClientSecret = "test-client-secret";
                settings.RedirectUri = new Uri("https://localhost/callback/login/kick");
            });
        });

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictClientOptions>>().CurrentValue;

        var registration = Assert.Single(options.Registrations);
        Assert.Equal("Kick", registration.ProviderName);
        Assert.Equal("test-client-id", registration.ClientId);
        Assert.Equal(new Uri("https://id.kick.com/"), registration.Issuer);
        Assert.Contains("user:read", registration.Scopes);

        var configuration = registration.Configuration!;
        Assert.Equal(new Uri("https://id.kick.com/oauth/authorize"), configuration.AuthorizationEndpoint);
        Assert.Equal(new Uri("https://id.kick.com/oauth/token"), configuration.TokenEndpoint);
        Assert.Equal(new Uri("https://api.kick.com/public/v1/users"), configuration.UserInfoEndpoint);
        Assert.Contains("S256", configuration.CodeChallengeMethodsSupported);
        Assert.Contains("client_secret_post", configuration.TokenEndpointAuthMethodsSupported);
    }

    [Fact]
    public async Task Challenge_Redirects_To_Kick_With_Pkce()
    {
        using var host = await new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.ConfigureServices(services =>
                {
                    services.AddOpenIddict().AddClient(options =>
                    {
                        options.AllowAuthorizationCodeFlow();
                        options.AddEphemeralEncryptionKey().AddEphemeralSigningKey();
                        options.DisableTokenStorage();
                        options.UseAspNetCore();
                        options.UseSystemNetHttp();

                        options.AddKick(settings =>
                        {
                            settings.ClientId = "test-client-id";
                            settings.ClientSecret = "test-client-secret";
                            settings.RedirectUri = new Uri("https://localhost/callback/login/kick");
                        });
                    });
                });
                webHost.Configure(app =>
                {
                    app.UseAuthentication();
                    app.Use(async (context, next) =>
                    {
                        if (context.Request.Path == "/challenge")
                        {
                            var properties = new AuthenticationProperties(new Dictionary<string, string?>
                            {
                                [OpenIddictClientAspNetCoreConstants.Properties.ProviderName] = KickOpenIddictClientDefaults.ProviderName
                            })
                            {
                                RedirectUri = "/"
                            };

                            await context.ChallengeAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme, properties);
                        }
                        else
                        {
                            await next();
                        }
                    });
                });
            })
            .StartAsync();

        host.GetTestServer().BaseAddress = new Uri("https://localhost");
        using var client = host.GetTestServer().CreateClient();

        using var response = await client.GetAsync("/challenge");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        var location = response.Headers.Location!;
        Assert.Equal("id.kick.com", location.Host);
        Assert.Equal("/oauth/authorize", location.AbsolutePath);

        var query = QueryHelpers.ParseQuery(location.Query);
        Assert.Equal("test-client-id", query["client_id"].ToString());
        Assert.Equal("code", query["response_type"].ToString());
        Assert.Equal("https://localhost/callback/login/kick", query["redirect_uri"].ToString());
        Assert.Equal("S256", query["code_challenge_method"].ToString());
        Assert.False(string.IsNullOrEmpty(query["code_challenge"].ToString()));
        Assert.False(string.IsNullOrEmpty(query["state"].ToString()));
        Assert.Contains("user:read", query["scope"].ToString());
    }
}
