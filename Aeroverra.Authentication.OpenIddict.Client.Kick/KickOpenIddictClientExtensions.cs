using Aeroverra.Authentication.OpenIddict.Client.Kick;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to register the Kick provider in the OpenIddict client.
/// </summary>
public static class KickOpenIddictClientExtensions
{
    /// <summary>
    /// Adds a client registration for Kick to the OpenIddict client, which
    /// enables Kick authentication capabilities.
    /// </summary>
    /// <param name="builder">The OpenIddict client builder.</param>
    /// <param name="configuration">The delegate used to configure the Kick settings.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static OpenIddictClientBuilder AddKick(
        this OpenIddictClientBuilder builder,
        Action<KickOpenIddictClientSettings> configuration)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);

        var settings = new KickOpenIddictClientSettings();
        configuration(settings);

        var registration = new OpenIddictClientRegistration
        {
            ProviderName = KickOpenIddictClientDefaults.ProviderName,
            ProviderDisplayName = KickOpenIddictClientDefaults.DisplayName,
            Issuer = new Uri(KickOpenIddictClientDefaults.Issuer, UriKind.Absolute),
            ClientId = settings.ClientId,
            ClientSecret = settings.ClientSecret,
            RedirectUri = settings.RedirectUri,

            // Kick doesn't expose an OpenID Connect discovery document,
            // so a static configuration is attached to the registration.
            Configuration = new OpenIddictConfiguration
            {
                Issuer = new Uri(KickOpenIddictClientDefaults.Issuer, UriKind.Absolute),
                AuthorizationEndpoint = new Uri(KickOpenIddictClientDefaults.AuthorizationEndpoint, UriKind.Absolute),
                TokenEndpoint = new Uri(KickOpenIddictClientDefaults.TokenEndpoint, UriKind.Absolute),
                UserInfoEndpoint = new Uri(KickOpenIddictClientDefaults.UserInformationEndpoint, UriKind.Absolute),
                GrantTypesSupported =
                {
                    GrantTypes.AuthorizationCode,
                    GrantTypes.RefreshToken
                },
                ResponseTypesSupported =
                {
                    ResponseTypes.Code
                },
                ResponseModesSupported =
                {
                    ResponseModes.Query
                },
                // Kick requires PKCE.
                CodeChallengeMethodsSupported =
                {
                    CodeChallengeMethods.Sha256
                },
                TokenEndpointAuthMethodsSupported =
                {
                    ClientAuthenticationMethods.ClientSecretPost
                }
            }
        };

        foreach (var scope in settings.Scopes)
        {
            registration.Scopes.Add(scope);
        }

        builder.AddRegistration(registration);
        builder.AddEventHandler(KickOpenIddictClientHandlers.NormalizeUserInfoResponse.Descriptor);

        return builder;
    }
}
