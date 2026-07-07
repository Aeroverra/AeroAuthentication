namespace Aeroverra.Authentication.OpenIddict.Client.Kick;

/// <summary>
/// Contains constants specific to the Kick OpenIddict client integration.
/// </summary>
public static class KickOpenIddictClientConstants
{
    /// <summary>
    /// The OAuth 2.0 scopes supported by Kick. Only <see cref="UserRead"/> is
    /// requested by default; add others through <see cref="KickOpenIddictClientSettings.Scopes"/>.
    /// </summary>
    public static class Scopes
    {
        /// <summary>
        /// View user information in Kick including username, streamer ID, etc.
        /// </summary>
        public const string UserRead = "user:read";

        /// <summary>
        /// View channel information in Kick including channel description, category, etc.
        /// </summary>
        public const string ChannelRead = "channel:read";

        /// <summary>
        /// Update livestream metadata for a channel.
        /// </summary>
        public const string ChannelWrite = "channel:write";

        /// <summary>
        /// Read channel points rewards information on a channel.
        /// </summary>
        public const string ChannelRewardsRead = "channel:rewards:read";

        /// <summary>
        /// Read, add, edit and delete channel points rewards on a channel.
        /// </summary>
        public const string ChannelRewardsWrite = "channel:rewards:write";

        /// <summary>
        /// Send chat messages and allow chat bots to post in your chat.
        /// </summary>
        public const string ChatWrite = "chat:write";

        /// <summary>
        /// Read a user's stream URL and stream key.
        /// </summary>
        public const string StreamKeyRead = "streamkey:read";

        /// <summary>
        /// Subscribe to all channel events on Kick, e.g. chat messages, follows, subscriptions.
        /// </summary>
        public const string EventsSubscribe = "events:subscribe";

        /// <summary>
        /// Execute moderation actions for moderators.
        /// </summary>
        public const string ModerationBan = "moderation:ban";

        /// <summary>
        /// Execute moderation actions on chat messages.
        /// </summary>
        public const string ModerationChatMessageManage = "moderation:chat_message:manage";

        /// <summary>
        /// View KICKs related information in Kick, e.g. leaderboards.
        /// </summary>
        public const string KicksRead = "kicks:read";
    }
}
