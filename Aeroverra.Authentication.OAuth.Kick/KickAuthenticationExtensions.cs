using Aeroverra.Authentication.OAuth.Kick;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to add Kick authentication capabilities to an HTTP application pipeline.
/// </summary>
public static class KickAuthenticationExtensions
{
    /// <summary>
    /// Adds <see cref="KickAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables Kick authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static AuthenticationBuilder AddKick(this AuthenticationBuilder builder)
    {
        return builder.AddKick(KickAuthenticationDefaults.AuthenticationScheme, static options => { });
    }

    /// <summary>
    /// Adds <see cref="KickAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables Kick authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="configuration">The delegate used to configure the Kick options.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static AuthenticationBuilder AddKick(
        this AuthenticationBuilder builder,
        Action<KickAuthenticationOptions> configuration)
    {
        return builder.AddKick(KickAuthenticationDefaults.AuthenticationScheme, configuration);
    }

    /// <summary>
    /// Adds <see cref="KickAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables Kick authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="scheme">The authentication scheme associated with this instance.</param>
    /// <param name="configuration">The delegate used to configure the Kick options.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static AuthenticationBuilder AddKick(
        this AuthenticationBuilder builder,
        string scheme,
        Action<KickAuthenticationOptions> configuration)
    {
        return builder.AddKick(scheme, KickAuthenticationDefaults.DisplayName, configuration);
    }

    /// <summary>
    /// Adds <see cref="KickAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables Kick authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="scheme">The authentication scheme associated with this instance.</param>
    /// <param name="caption">The display name associated with this instance.</param>
    /// <param name="configuration">The delegate used to configure the Kick options.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static AuthenticationBuilder AddKick(
        this AuthenticationBuilder builder,
        string scheme,
        string caption,
        Action<KickAuthenticationOptions> configuration)
    {
        return builder.AddOAuth<KickAuthenticationOptions, KickAuthenticationHandler>(scheme, caption, configuration);
    }
}
