namespace Aeroverra.Authentication.OpenIddict.Client.Kick;

/// <summary>
/// Defines the settings used to register the Kick provider in the OpenIddict client.
/// </summary>
public sealed class KickOpenIddictClientSettings
{
    /// <summary>
    /// Gets or sets the client identifier issued by Kick.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret issued by Kick.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the redirect URI registered with Kick.
    /// </summary>
    public Uri? RedirectUri { get; set; }

    /// <summary>
    /// Gets the scopes requested from Kick. Defaults to <c>user:read</c>.
    /// </summary>
    public IList<string> Scopes { get; } = ["user:read"];
}
