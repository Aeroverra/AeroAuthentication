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
`KickAuthenticationConstants.Claims.ProfileImageUrl`.
