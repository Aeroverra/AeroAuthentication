using Aeroverra.Authentication.OpenIddict.Client.Aero;
using OpenIddict.Client;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to register the Aero.VI provider in the OpenIddict client.
/// </summary>
public static class AeroOpenIddictClientExtensions
{
    /// <summary>
    /// Adds a client registration for Aero.VI to the OpenIddict client, which
    /// enables Aero.VI authentication capabilities. The endpoints are resolved
    /// automatically from the OpenID Connect discovery document exposed by Aero.VI.
    /// </summary>
    /// <param name="builder">The OpenIddict client builder.</param>
    /// <param name="configuration">The delegate used to configure the Aero.VI settings.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static OpenIddictClientBuilder AddAero(
        this OpenIddictClientBuilder builder,
        Action<AeroOpenIddictClientSettings> configuration)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);

        var settings = new AeroOpenIddictClientSettings();
        configuration(settings);

        var registration = new OpenIddictClientRegistration
        {
            ProviderName = AeroOpenIddictClientDefaults.ProviderName,
            ProviderDisplayName = AeroOpenIddictClientDefaults.DisplayName,
            Issuer = new Uri(AeroOpenIddictClientDefaults.Issuer, UriKind.Absolute),
            ClientId = settings.ClientId,
            ClientSecret = settings.ClientSecret,
            RedirectUri = settings.RedirectUri
        };

        foreach (var scope in settings.Scopes)
        {
            registration.Scopes.Add(scope);
        }

        builder.AddRegistration(registration);

        return builder;
    }
}
