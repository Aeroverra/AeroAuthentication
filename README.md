# Aero Authentication

[![Build](https://github.com/Aeroverra/AeroAuthentication/actions/workflows/nuget-publish.yml/badge.svg)](https://github.com/Aeroverra/AeroAuthentication/actions/workflows/nuget-publish.yml)[![NuGet](https://img.shields.io/nuget/v/Aeroverra.Authentication.OAuth.Kick.svg?style=flat&label=OAuth.Kick)](https://www.nuget.org/packages/Aeroverra.Authentication.OAuth.Kick)[![NuGet](https://img.shields.io/nuget/v/Aeroverra.Authentication.OAuth.Aero.svg?style=flat&label=OAuth.Aero)](https://www.nuget.org/packages/Aeroverra.Authentication.OAuth.Aero)

Authentication providers for ASP.NET Core, available in two flavors per provider:

- **OAuth handlers** built on `Microsoft.AspNetCore.Authentication.OAuth`, following the conventions of
  [AspNet.Security.OAuth.Providers](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers).
- **OpenIddict client registrations** for apps using the [OpenIddict](https://github.com/openiddict/openiddict-core) client stack.

Each provider and flavor ships as its own NuGet package.

## Packages

| Provider | ASP.NET Core OAuth | OpenIddict client | Documentation |
| -------- | ------------------ | ----------------- | ------------- |
| [Kick](https://kick.com) | [Aeroverra.Authentication.OAuth.Kick](https://www.nuget.org/packages/Aeroverra.Authentication.OAuth.Kick) | Aeroverra.Authentication.OpenIddict.Client.Kick (not yet published) | [docs/kick.md](docs/kick.md) |
| [Aero.VI](https://aero.vi) | [Aeroverra.Authentication.OAuth.Aero](https://www.nuget.org/packages/Aeroverra.Authentication.OAuth.Aero) | Aeroverra.Authentication.OpenIddict.Client.Aero (not yet published) | [docs/aero.md](docs/aero.md) |

### Naming convention

Package IDs follow `Aeroverra.Authentication.<Stack>.<Provider>`, where the stack segment names the
authentication stack the package extends, spelled the way that stack spells itself:

- `OAuth` extends the ASP.NET Core OAuth handler stack (`Microsoft.AspNetCore.Authentication.OAuth`).
  This stack is inherently client-side, so no extra qualifier is needed.
- `OpenIddict.Client` extends the OpenIddict client stack (`OpenIddict.Client`), mirroring OpenIddict's
  own package family. This leaves room for future `OpenIddict.Server` or `JwtBearer` packages without
  renaming anything.

The root namespace and assembly name of every project match its package ID.

## Requirements

- .NET 10 (`net10.0`)
- The OAuth packages reference the `Microsoft.AspNetCore.App` shared framework
- The OpenIddict packages depend on `OpenIddict.Client` 7.5.0 or later

## Usage: OAuth handler

The OAuth handlers plug into the standard ASP.NET Core authentication pipeline, exactly like the
built-in Google or Facebook providers:

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
    })
    .AddAero(options =>
    {
        options.ClientId = builder.Configuration["Aero:ClientId"]!;
        options.ClientSecret = builder.Configuration["Aero:ClientSecret"]!;
    });
```

Defaults per provider (endpoints, callback paths, scopes, PKCE, claim mappings) are listed in the
per-provider docs: [Kick](docs/kick.md), [Aero.VI](docs/aero.md).

## Usage: OpenIddict client

The OpenIddict packages add a preconfigured `OpenIddictClientRegistration` per provider:

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

        options.AddAero(settings =>
        {
            settings.ClientId = builder.Configuration["Aero:ClientId"]!;
            settings.ClientSecret = builder.Configuration["Aero:ClientSecret"]!;
            settings.RedirectUri = new Uri("https://your-site/callback/login/aero");
        });
    });
```

Provider-specific behavior:

- **Kick** has no OpenID Connect discovery document, so the registration carries a static
  configuration (authorization code + refresh token grants, S256 PKCE, `client_secret_post`).
  A custom event handler unwraps Kick's `{"data":[...]}` userinfo envelope and maps `user_id`
  and `profile_picture` to the standard `sub` and `picture` claims.
- **Aero.VI** exposes a discovery document at `https://accounts.aero.vi/.well-known/openid-configuration`,
  so the registration only needs the issuer, credentials, and scopes; everything else is resolved
  automatically.

## Repository layout

```
Aeroverra.Authentication.OAuth.Kick/               OAuth handler package (Kick)
Aeroverra.Authentication.OAuth.Aero/               OAuth handler package (Aero.VI)
Aeroverra.Authentication.OpenIddict.Client.Kick/   OpenIddict client package (Kick)
Aeroverra.Authentication.OpenIddict.Client.Aero/   OpenIddict client package (Aero.VI)
Aeroverra.Authentication.Tests/                    Test suite for all packages
docs/                                              Per-provider documentation
Directory.Build.props                              Shared VersionPrefix (single source of truth)
.github/workflows/nuget-publish.yml                Build, test, pack, and publish pipeline
```

Each OAuth provider project follows the aspnet-contrib five-file pattern
(`<Provider>AuthenticationConstants/Defaults/Extensions/Handler/Options.cs`) and its current code
style (file-scoped namespaces, primary constructors, `[LoggerMessage]` logging, `AddX()` extensions
in the `Microsoft.Extensions.DependencyInjection` namespace), so a provider can be contributed
upstream with minimal changes.

## Building and testing

```
dotnet build Aeroverra.Authentication.slnx
dotnet test Aeroverra.Authentication.slnx
```

The build must complete with zero warnings. The tests run entirely in memory with no network access:

- OAuth handlers: a full authorization code flow against a `TestServer` (challenge, callback with
  correlation cookies, token exchange and userinfo against a stubbed backchannel, cookie sign-in),
  asserting every mapped claim.
- OpenIddict client: registration content assertions plus a real challenge through the OpenIddict
  ASP.NET Core host, asserting the produced authorization URL (endpoints, PKCE, scopes, state).

## Versioning and publishing

- All packages share one version: `<major.minor>` comes from the `VersionPrefix` in
  `Directory.Build.props` and the patch is the CI run number. Bump `VersionPrefix` to change
  the major/minor line.
- Every push to `main` triggers [nuget-publish.yml](.github/workflows/nuget-publish.yml):
  restore, build, test, pack the solution, push to NuGet, then tag the commit `v<version>`.
  A manual run (`workflow_dispatch`) accepts an explicit version override.
- Publishing uses NuGet trusted publishing (OIDC via `NuGet/login`, no stored API key) under the
  GitHub environment `Production`. The nuget.org policy must reference the repository
  `Aeroverra/AeroAuthentication`, the workflow file `nuget-publish.yml`, and the environment
  `Production`.
- A project is published if and only if it is packable: projects marked `<IsPackable>false</IsPackable>`
  (currently both OpenIddict packages and the test project) are built and tested but never packed
  or pushed. Flip that property to include a package in the next release.

## Adding a new provider

1. Create `Aeroverra.Authentication.OAuth.<Provider>/` by copying an existing provider project and
   adjusting the five files (defaults, options, handler, extensions, constants if custom claims).
2. Add tests in `Aeroverra.Authentication.Tests` (defaults, challenge redirect, full sign-in flow
   with a stubbed backchannel).
3. Add `docs/<provider>.md` and a row to the packages table above.
4. Add the project to `Aeroverra.Authentication.slnx`.
5. Optionally add the OpenIddict flavor as `Aeroverra.Authentication.OpenIddict.Client.<Provider>`
   (static configuration if the provider lacks OIDC discovery, plus userinfo normalization handlers
   if its payload is non-standard).
6. Before it can publish, add trusted publishing coverage for the new package ID on nuget.org.

## License

MIT. See [LICENSE.md](LICENSE.md).
