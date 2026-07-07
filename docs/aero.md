# Integrating the Aero.VI Provider

## Setup

Register an OAuth application with [Aero.VI](https://aero.vi) and add
`https://your-site/signin-aero` as an allowed redirect URI.

## Example

```csharp
services.AddAuthentication()
        .AddAero(options =>
        {
            options.ClientId = "your-client-id";
            options.ClientSecret = "your-client-secret";
        });
```

## Defaults

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
