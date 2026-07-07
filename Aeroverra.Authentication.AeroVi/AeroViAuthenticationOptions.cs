using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Aeroverra.Authentication.AeroVi;

/// <summary>
/// Defines a set of options used by <see cref="AeroViAuthenticationHandler"/>.
/// </summary>
public class AeroViAuthenticationOptions : OAuthOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AeroViAuthenticationOptions"/> class.
    /// </summary>
    public AeroViAuthenticationOptions()
    {
        ClaimsIssuer = AeroViAuthenticationDefaults.Issuer;
        CallbackPath = AeroViAuthenticationDefaults.CallbackPath;

        AuthorizationEndpoint = AeroViAuthenticationDefaults.AuthorizationEndpoint;
        TokenEndpoint = AeroViAuthenticationDefaults.TokenEndpoint;
        UserInformationEndpoint = AeroViAuthenticationDefaults.UserInformationEndpoint;

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
