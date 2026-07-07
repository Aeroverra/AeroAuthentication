# Aeroverra.Authentication.OAuth.Kick

ASP.NET Core security middleware enabling [Kick](https://kick.com) OAuth 2.0 authentication,
following the conventions of
[AspNet.Security.OAuth.Providers](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers).

## Install

```
dotnet add package Aeroverra.Authentication.OAuth.Kick
```

## Usage

Create an application in the [Kick developer settings](https://kick.com/settings/developer) and add
`https://your-site/signin-kick` as an allowed redirect URI, then:

```csharp
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddKick(options =>
    {
        options.ClientId = builder.Configuration["Kick:ClientId"]!;
        options.ClientSecret = builder.Configuration["Kick:ClientSecret"]!;
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

Every scope supported by Kick (channel, chat, rewards, moderation, events, etc.) is available
as a constant on `KickAuthenticationConstants.Scopes`; add the ones you need through
`options.Scope.Add(...)`.

## Claims

| Claim | JSON field |
| ----- | ---------- |
| `ClaimTypes.NameIdentifier` | `user_id` |
| `ClaimTypes.Name` | `name` |
| `ClaimTypes.Email` | `email` |
| `urn:kick:profilepicture` (`KickAuthenticationConstants.Claims.ProfileImageUrl`) | `profile_picture` |

## More

Full documentation and source: [Aeroverra/AeroAuthentication](https://github.com/Aeroverra/AeroAuthentication).
An OpenIddict client flavor of this provider is also available in the same repository.
