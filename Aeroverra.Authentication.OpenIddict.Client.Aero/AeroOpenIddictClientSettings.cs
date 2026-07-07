using OpenIddict.Abstractions;

namespace Aeroverra.Authentication.OpenIddict.Client.Aero;

/// <summary>
/// Defines the settings used to register the Aero.VI provider in the OpenIddict client.
/// </summary>
public sealed class AeroOpenIddictClientSettings
{
    /// <summary>
    /// Gets or sets the client identifier issued by Aero.VI.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret issued by Aero.VI.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the redirect URI registered with Aero.VI.
    /// </summary>
    public Uri? RedirectUri { get; set; }

    /// <summary>
    /// Gets the scopes requested from Aero.VI. Defaults to <c>openid</c> (required for
    /// Aero.VI to issue an identity token), <c>offline_access</c> and <c>user.profile.read</c>.
    /// </summary>
    public IList<string> Scopes { get; } =
    [
        OpenIddictConstants.Scopes.OpenId,
        AeroOpenIddictClientConstants.Scopes.OfflineAccess,
        AeroOpenIddictClientConstants.Scopes.UserProfileRead
    ];
}
