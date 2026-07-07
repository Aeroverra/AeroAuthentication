# Integrating the Aero.VI Provider

## Setup

Register an OAuth application with [Aero.VI](https://aero.vi) and add your redirect URI
(`https://your-site/signin-aero` for the OAuth handler) as an allowed redirect URI.

## Example (OAuth handler, package `Aeroverra.Authentication.OAuth.Aero`)

```csharp
services.AddAuthentication()
        .AddAero(options =>
        {
            options.ClientId = "your-client-id";
            options.ClientSecret = "your-client-secret";
        });
```

## Example (OpenIddict client, package `Aeroverra.Authentication.OpenIddict.Client.Aero`)

Aero.VI exposes an OpenID Connect discovery document at
`https://api.aero.vi/.well-known/openid-configuration`, so the endpoints are resolved
automatically from the issuer. The default scopes are `openid`, `offline_access` and
`user.profile.read`.

```csharp
services.AddOpenIddict()
        .AddClient(options =>
        {
            options.AllowAuthorizationCodeFlow();
            options.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate();
            options.UseAspNetCore().EnableRedirectionEndpointPassthrough();
            options.UseSystemNetHttp();

            options.AddAero(settings =>
            {
                settings.ClientId = "your-client-id";
                settings.ClientSecret = "your-client-secret";
                settings.RedirectUri = new Uri("https://your-site/callback/login/aero");
            });
        });
```

## OAuth handler defaults

| Setting | Value |
| ------- | ----- |
| Authentication scheme | `Aero` |
| Callback path | `/signin-aero` |
| Authorization endpoint | `https://api.aero.vi/oauth/authorize` |
| Token endpoint | `https://api.aero.vi/oauth/token` |
| User information endpoint | `https://api.aero.vi/user/userinfo` |
| Scopes | `offline_access`, `user.profile.read` |
| PKCE | Required by Aero.VI, enabled by default |

## Claims

| Claim | JSON field |
| ----- | ---------- |
| `ClaimTypes.NameIdentifier` | `id` |
| `ClaimTypes.Name` | `userName` |
| `ClaimTypes.Email` | `email` |

## Scopes

The scopes used by the Aero.VI provider are exposed as constants on
`AeroAuthenticationConstants.Scopes` (OAuth handler) and
`AeroOpenIddictClientConstants.Scopes` (OpenIddict client):

| Scope | Constant | Grants |
| ----- | -------- | ------ |
| `offline_access` | `Scopes.OfflineAccess` | Allows Aero.VI to issue refresh tokens |
| `user.profile.read` | `Scopes.UserProfileRead` | Basic profile information (name, username, etc.) |

The OAuth handler requests both by default. The OpenIddict client additionally requests the
standard `openid` scope, which Aero.VI needs to issue the identity token that carries the
user's identity.
