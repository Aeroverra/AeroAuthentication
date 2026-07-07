# Aeroverra.Authentication.OpenIddict.Client.Kick

[OpenIddict](https://github.com/openiddict/openiddict-core) client integration enabling
[Kick](https://kick.com) OAuth 2.0 authentication.

Kick doesn't expose an OpenID Connect discovery document, so this package attaches a static
configuration to the client registration (authorization code + refresh token grants, S256 PKCE,
`client_secret_post`). A custom event handler unwraps Kick's `{"data":[...]}` userinfo envelope
and maps `user_id` and `profile_picture` to the standard `sub` and `picture` claims.

## Install

```
dotnet add package Aeroverra.Authentication.OpenIddict.Client.Kick
```

## Usage

```csharp
builder.Services
    .AddOpenIddict()
    .AddClient(options =>
    {
        options.AllowAuthorizationCodeFlow();
        options.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate();
        options.UseAspNetCore().EnableRedirectionEndpointPassthrough();
        options.UseSystemNetHttp();

        options.AddKick(settings =>
        {
            settings.ClientId = builder.Configuration["Kick:ClientId"]!;
            settings.ClientSecret = builder.Configuration["Kick:ClientSecret"]!;
            settings.RedirectUri = new Uri("https://your-site/callback/login/kick");
        });
    });
```

The default scope is `user:read`; every scope supported by Kick is available as a constant on
`KickOpenIddictClientConstants.Scopes`, and you can add them through `settings.Scopes`.

## More

Full documentation and source: [Aeroverra/AeroAuthentication](https://github.com/Aeroverra/AeroAuthentication).
A plain ASP.NET Core OAuth handler flavor of this provider is also available in the same repository.
