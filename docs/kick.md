# Integrating the Kick Provider

## Setup

Create an application in the [Kick developer settings](https://kick.com/settings/developer) and add
`https://your-site/signin-kick` as an allowed redirect URI. See the
[Kick developer documentation](https://docs.kick.com/) for details.

## Example

```csharp
services.AddAuthentication()
        .AddKick(options =>
        {
            options.ClientId = "your-client-id";
            options.ClientSecret = "your-client-secret";
        });
```

## Defaults

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
`KickAuthenticationConstants.Claims.ProfileImageUrl`.
