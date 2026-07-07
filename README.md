# Aero Authentication

Authentication providers for ASP.NET Core, available in two flavors per provider:

- **OAuth handlers** built on `Microsoft.AspNetCore.Authentication.OAuth`, following the conventions of
  [AspNet.Security.OAuth.Providers](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers).
- **OpenIddict client registrations** for apps using the [OpenIddict](https://github.com/openiddict/openiddict-core) client stack.

Each provider and flavor ships as its own NuGet package.

## Packages

| Provider | ASP.NET Core OAuth | OpenIddict client | Documentation |
| -------- | ------------------ | ----------------- | ------------- |
| [Kick](https://kick.com) | [Aeroverra.Authentication.OAuth.Kick](https://www.nuget.org/packages/Aeroverra.Authentication.OAuth.Kick) | [Aeroverra.Authentication.OpenIddict.Client.Kick](https://www.nuget.org/packages/Aeroverra.Authentication.OpenIddict.Client.Kick) | [docs/kick.md](docs/kick.md) |
| [Aero.VI](https://aero.vi) | [Aeroverra.Authentication.OAuth.Aero](https://www.nuget.org/packages/Aeroverra.Authentication.OAuth.Aero) | [Aeroverra.Authentication.OpenIddict.Client.Aero](https://www.nuget.org/packages/Aeroverra.Authentication.OpenIddict.Client.Aero) | [docs/aero.md](docs/aero.md) |

## Getting started (OAuth handler)

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

## Getting started (OpenIddict client)

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

See the per-provider documentation in [docs](docs) for endpoints, scopes, and the claims each provider maps.

## Building

```
dotnet build Aeroverra.Authentication.slnx
dotnet test Aeroverra.Authentication.slnx
```

Packages are published automatically to NuGet by the [nuget-publish](.github/workflows/nuget-publish.yml)
workflow on every push to `main`.

## License

MIT. See [LICENSE.md](LICENSE.md).
