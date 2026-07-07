using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using static Aeroverra.Authentication.OAuth.Kick.KickAuthenticationConstants;

namespace Aeroverra.Authentication.OAuth.Kick;

/// <summary>
/// Defines a set of options used by <see cref="KickAuthenticationHandler"/>.
/// </summary>
public class KickAuthenticationOptions : OAuthOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KickAuthenticationOptions"/> class.
    /// </summary>
    public KickAuthenticationOptions()
    {
        ClaimsIssuer = KickAuthenticationDefaults.Issuer;
        CallbackPath = KickAuthenticationDefaults.CallbackPath;

        AuthorizationEndpoint = KickAuthenticationDefaults.AuthorizationEndpoint;
        TokenEndpoint = KickAuthenticationDefaults.TokenEndpoint;
        UserInformationEndpoint = KickAuthenticationDefaults.UserInformationEndpoint;

        // View user information in Kick including username, streamer ID, etc.
        Scope.Add(Scopes.UserRead);

        // Kick requires PKCE.
        UsePkce = true;

        ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "user_id");
        ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        ClaimActions.MapJsonKey(Claims.ProfileImageUrl, "profile_picture");
    }
}
