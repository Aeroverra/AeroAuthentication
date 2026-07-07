# Integrating the Kick Provider

## Setup

Create an application in the [Kick developer settings](https://kick.com/settings/developer) and add
your redirect URI (`https://your-site/signin-kick` for the OAuth handler) as an allowed redirect URI.
See the [Kick developer documentation](https://docs.kick.com/) for details.

## Example (OAuth handler, package `Aeroverra.Authentication.OAuth.Kick`)

```csharp
services.AddAuthentication()
        .AddKick(options =>
        {
            options.ClientId = "your-client-id";
            options.ClientSecret = "your-client-secret";
        });
```

## Example (OpenIddict client, package `Aeroverra.Authentication.OpenIddict.Client.Kick`)

Kick doesn't expose an OpenID Connect discovery document, so the registration attaches a static
configuration. A custom event handler unwraps Kick's `data` envelope and maps `user_id` and
`profile_picture` to the standard `sub` and `picture` claims.

```csharp
services.AddOpenIddict()
        .AddClient(options =>
        {
            options.AllowAuthorizationCodeFlow();
            options.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate();
            options.UseAspNetCore().EnableRedirectionEndpointPassthrough();
            options.UseSystemNetHttp();

            options.AddKick(settings =>
            {
                settings.ClientId = "your-client-id";
                settings.ClientSecret = "your-client-secret";
                settings.RedirectUri = new Uri("https://your-site/callback/login/kick");
            });
        });
```

## OAuth handler defaults

| Setting | Value |
| ------- | ----- |
| Authentication scheme | `Kick` |
| Callback path | `/signin-kick` |
| Authorization endpoint | `https://id.kick.com/oauth/authorize` |
| Token endpoint | `https://id.kick.com/oauth/token` |
| User information endpoint | `https://api.kick.com/public/v1/users` |
| Scopes | `user:read` |
| PKCE | Required by Kick, enabled by default |

## Claims

| Claim | JSON field |
| ----- | ---------- |
| `ClaimTypes.NameIdentifier` | `user_id` |
| `ClaimTypes.Name` | `name` |
| `ClaimTypes.Email` | `email` |
| `urn:kick:profilepicture` | `profile_picture` |

The profile picture claim type is available as
`KickAuthenticationConstants.Claims.ProfileImageUrl`. Note that this is a claim type
(the key the value is stored under on the signed-in `ClaimsPrincipal`), not a scope;
it is never sent to Kick.

## Scopes

Only `user:read` is requested by default. All scopes supported by Kick are available as
constants on `KickAuthenticationConstants.Scopes` (OAuth handler) and
`KickOpenIddictClientConstants.Scopes` (OpenIddict client):

| Scope | Constant | Grants |
| ----- | -------- | ------ |
| `user:read` | `Scopes.UserRead` | View user information including username, streamer ID, etc. |
| `channel:read` | `Scopes.ChannelRead` | View channel information including description, category, etc. |
| `channel:write` | `Scopes.ChannelWrite` | Update livestream metadata for a channel |
| `channel:rewards:read` | `Scopes.ChannelRewardsRead` | Read channel points rewards on a channel |
| `channel:rewards:write` | `Scopes.ChannelRewardsWrite` | Read, add, edit and delete channel points rewards |
| `chat:write` | `Scopes.ChatWrite` | Send chat messages and allow chat bots to post |
| `streamkey:read` | `Scopes.StreamKeyRead` | Read a user's stream URL and stream key |
| `events:subscribe` | `Scopes.EventsSubscribe` | Subscribe to channel events (chat messages, follows, subscriptions, ...) |
| `moderation:ban` | `Scopes.ModerationBan` | Execute moderation actions for moderators |
| `moderation:chat_message:manage` | `Scopes.ModerationChatMessageManage` | Execute moderation actions on chat messages |
| `kicks:read` | `Scopes.KicksRead` | View KICKs related information, e.g. leaderboards |

```csharp
.AddKick(options =>
{
    options.ClientId = "your-client-id";
    options.ClientSecret = "your-client-secret";
    options.Scope.Add(KickAuthenticationConstants.Scopes.ChannelRead);
    options.Scope.Add(KickAuthenticationConstants.Scopes.ChatWrite);
});
```
