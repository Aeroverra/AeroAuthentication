# Aeroverra.Authentication.OAuth.Aero

ASP.NET Core security middleware enabling [Aero.VI](https://aero.vi) OAuth 2.0 authentication,
following the conventions of
[AspNet.Security.OAuth.Providers](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers).

## Install

```
dotnet add package Aeroverra.Authentication.OAuth.Aero
```

## Usage

Register an OAuth application with [Aero.VI](https://aero.vi) and add
`https://your-site/signin-aero` as an allowed redirect URI, then:

```csharp
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddAero(options =>
    {
        options.ClientId = builder.Configuration["Aero:ClientId"]!;
        options.ClientSecret = builder.Configuration["Aero:ClientSecret"]!;
    });
```

## Defaults

| Setting | Value |
| ------- | ----- |
| Authentication scheme | `Aero` |
| Callback path | `/signin-aero` |
| Authorization endpoint | `https://accounts.aero.vi/oauth/authorize` |
| Token endpoint | `https://accounts.aero.vi/oauth/token` |
| User information endpoint | `https://accounts.aero.vi/user/userinfo` |
| Scopes | `offline_access`, `user.profile.read` |
| PKCE | Required by Aero.VI, enabled by default |

Both scopes are available as constants on `AeroAuthenticationConstants.Scopes`.

## Claims

| Claim | JSON field |
| ----- | ---------- |
| `ClaimTypes.NameIdentifier` | `id` |
| `ClaimTypes.Name` | `userName` |
| `ClaimTypes.Email` | `email` |

## More

Full documentation and source: [Aeroverra/AeroAuthentication](https://github.com/Aeroverra/AeroAuthentication).
An OpenIddict client flavor of this provider is also available in the same repository.
