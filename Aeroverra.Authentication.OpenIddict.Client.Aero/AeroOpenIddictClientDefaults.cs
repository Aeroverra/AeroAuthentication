namespace Aeroverra.Authentication.OpenIddict.Client.Aero;

/// <summary>
/// Default values used by the Aero.VI OpenIddict client integration.
/// </summary>
public static class AeroOpenIddictClientDefaults
{
    /// <summary>
    /// Default value for <see cref="global::OpenIddict.Client.OpenIddictClientRegistration.ProviderName"/>.
    /// </summary>
    public const string ProviderName = "Aero";

    /// <summary>
    /// Default value for <see cref="global::OpenIddict.Client.OpenIddictClientRegistration.ProviderDisplayName"/>.
    /// </summary>
    public const string DisplayName = "Aero";

    /// <summary>
    /// Default value for <see cref="global::OpenIddict.Client.OpenIddictClientRegistration.Issuer"/>.
    /// Aero.VI exposes an OpenID Connect discovery document, so the endpoints
    /// are resolved automatically from the issuer.
    /// </summary>
    public const string Issuer = "https://accounts.aero.vi/";
}
