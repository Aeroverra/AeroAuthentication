namespace Aeroverra.Authentication.OAuth.Kick;

/// <summary>
/// Contains constants specific to the <see cref="KickAuthenticationHandler"/>.
/// </summary>
public static class KickAuthenticationConstants
{
    /// <summary>
    /// Custom claim types issued by the Kick provider.
    /// </summary>
    public static class Claims
    {
        /// <summary>
        /// The claim holding the URL of the user's Kick profile picture.
        /// </summary>
        public const string ProfileImageUrl = "urn:kick:profilepicture";
    }
}
