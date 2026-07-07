# Aero Authentication

OAuth 2.0 authentication providers for ASP.NET Core, following the conventions of
[AspNet.Security.OAuth.Providers](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers).
Each provider ships as its own NuGet package.

## Providers

| Provider | Package | Documentation |
| -------- | ------- | ------------- |
| [Kick](https://kick.com) | [Aeroverra.Authentication.Kick](https://www.nuget.org/packages/Aeroverra.Authentication.Kick) | [docs/kick.md](docs/kick.md) |
| [Aero.VI](https://aero.vi) | [Aeroverra.Authentication.Aero](https://www.nuget.org/packages/Aeroverra.Authentication.Aero) | [docs/aero.md](docs/aero.md) |

## Getting started

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
