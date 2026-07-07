using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Aeroverra.Authentication.OAuth.Aero;

/// <summary>
/// Defines a set of options used by <see cref="AeroAuthenticationHandler"/>.
/// </summary>
public class AeroAuthenticationOptions : OAuthOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AeroAuthenticationOptions"/> class.
    /// </summary>
    public AeroAuthenticationOptions()
    {
        ClaimsIssuer = AeroAuthenticationDefaults.Issuer;
        CallbackPath = AeroAuthenticationDefaults.CallbackPath;

        AuthorizationEndpoint = AeroAuthenticationDefaults.AuthorizationEndpoint;
        TokenEndpoint = AeroAuthenticationDefaults.TokenEndpoint;
        UserInformationEndpoint = AeroAuthenticationDefaults.UserInformationEndpoint;

        // Allow Aero.VI to issue refresh tokens.
        Scope.Add("offline_access");

        // View user information in Aero.VI including username, email, etc.
        Scope.Add("user.profile.read");

        // Aero.VI requires PKCE.
        UsePkce = true;

        ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
        ClaimActions.MapJsonKey(ClaimTypes.Name, "userName");
        ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
    }
}
