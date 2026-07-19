# Aeroverra.Authentication.OpenIddict.Client.Aero

[OpenIddict](https://github.com/openiddict/openiddict-core) client integration enabling
[Aero.VI](https://aero.vi) OpenID Connect authentication.

Aero.VI exposes an OpenID Connect discovery document at
`https://accounts.aero.vi/.well-known/openid-configuration`, so the registration only needs the issuer,
credentials, and scopes; endpoints and signing keys are resolved automatically.

## Install

```
dotnet add package Aeroverra.Authentication.OpenIddict.Client.Aero
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

        options.AddAero(settings =>
        {
            settings.ClientId = builder.Configuration["Aero:ClientId"]!;
            settings.ClientSecret = builder.Configuration["Aero:ClientSecret"]!;
            settings.RedirectUri = new Uri("https://your-site/callback/login/aero");
        });
    });
```

The default scopes are `openid` (required for Aero.VI to issue an identity token),
`offline_access` and `user.profile.read`. The Aero.VI scopes are available as constants on
`AeroOpenIddictClientConstants.Scopes`, and you can adjust the list through `settings.Scopes`.

## More

Full documentation and source: [Aeroverra/AeroAuthentication](https://github.com/Aeroverra/AeroAuthentication).
A plain ASP.NET Core OAuth handler flavor of this provider is also available in the same repository.
