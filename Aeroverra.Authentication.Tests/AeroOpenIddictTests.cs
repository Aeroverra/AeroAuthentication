using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenIddict.Client;

namespace Aeroverra.Authentication.Tests;

public class AeroOpenIddictTests
{
    [Fact]
    public void Registration_Uses_Discovery_For_Aero()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOpenIddict().AddClient(options =>
        {
            options.AllowAuthorizationCodeFlow();
            options.AddEphemeralEncryptionKey().AddEphemeralSigningKey();
            options.UseAspNetCore();
            options.UseSystemNetHttp();

            options.AddAero(settings =>
            {
                settings.ClientId = "test-client-id";
                settings.ClientSecret = "test-client-secret";
                settings.RedirectUri = new Uri("https://localhost/callback/login/aero");
            });
        });

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictClientOptions>>().CurrentValue;

        var registration = Assert.Single(options.Registrations);
        Assert.Equal("Aero", registration.ProviderName);
        Assert.Equal("test-client-id", registration.ClientId);
        Assert.Equal(new Uri("https://accounts.aero.vi/"), registration.Issuer);

        // No static configuration: the endpoints are resolved from the discovery document.
        Assert.Null(registration.Configuration);

        Assert.Contains("openid", registration.Scopes);
        Assert.Contains("offline_access", registration.Scopes);
        Assert.Contains("user.profile.read", registration.Scopes);
    }
}
