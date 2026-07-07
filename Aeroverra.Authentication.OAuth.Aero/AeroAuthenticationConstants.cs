namespace Aeroverra.Authentication.OAuth.Aero;

/// <summary>
/// Contains constants specific to the <see cref="AeroAuthenticationHandler"/>.
/// </summary>
public static class AeroAuthenticationConstants
{
    /// <summary>
    /// The OAuth 2.0 scopes used by the Aero.VI provider. Both are requested by default.
    /// </summary>
    public static class Scopes
    {
        /// <summary>
        /// Allows Aero.VI to issue refresh tokens.
        /// </summary>
        public const string OfflineAccess = "offline_access";

        /// <summary>
        /// Access the user's basic profile information (name, username, etc.).
        /// </summary>
        public const string UserProfileRead = "user.profile.read";
    }
}
