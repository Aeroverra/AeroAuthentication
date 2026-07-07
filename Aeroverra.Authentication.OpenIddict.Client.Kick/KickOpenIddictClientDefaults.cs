namespace Aeroverra.Authentication.OpenIddict.Client.Kick;

/// <summary>
/// Default values used by the Kick OpenIddict client integration.
/// </summary>
public static class KickOpenIddictClientDefaults
{
    /// <summary>
    /// Default value for <see cref="global::OpenIddict.Client.OpenIddictClientRegistration.ProviderName"/>.
    /// </summary>
    public const string ProviderName = "Kick";

    /// <summary>
    /// Default value for <see cref="global::OpenIddict.Client.OpenIddictClientRegistration.ProviderDisplayName"/>.
    /// </summary>
    public const string DisplayName = "Kick";

    /// <summary>
    /// Default value for <see cref="global::OpenIddict.Client.OpenIddictClientRegistration.Issuer"/>.
    /// </summary>
    public const string Issuer = "https://id.kick.com/";

    /// <summary>
    /// The URI of the Kick authorization endpoint.
    /// </summary>
    public const string AuthorizationEndpoint = "https://id.kick.com/oauth/authorize";

    /// <summary>
    /// The URI of the Kick token endpoint.
    /// </summary>
    public const string TokenEndpoint = "https://id.kick.com/oauth/token";

    /// <summary>
    /// The URI of the Kick user information endpoint.
    /// </summary>
    public const string UserInformationEndpoint = "https://api.kick.com/public/v1/users";
}
