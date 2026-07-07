using Aeroverra.Authentication.Aero;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to add Aero.VI authentication capabilities to an HTTP application pipeline.
/// </summary>
public static class AeroAuthenticationExtensions
{
    /// <summary>
    /// Adds <see cref="AeroAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables Aero.VI authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static AuthenticationBuilder AddAero(this AuthenticationBuilder builder)
    {
        return builder.AddAero(AeroAuthenticationDefaults.AuthenticationScheme, static options => { });
    }

    /// <summary>
    /// Adds <see cref="AeroAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables Aero.VI authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="configuration">The delegate used to configure the Aero.VI options.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static AuthenticationBuilder AddAero(
        this AuthenticationBuilder builder,
        Action<AeroAuthenticationOptions> configuration)
    {
        return builder.AddAero(AeroAuthenticationDefaults.AuthenticationScheme, configuration);
    }

    /// <summary>
    /// Adds <see cref="AeroAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables Aero.VI authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="scheme">The authentication scheme associated with this instance.</param>
    /// <param name="configuration">The delegate used to configure the Aero.VI options.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static AuthenticationBuilder AddAero(
        this AuthenticationBuilder builder,
        string scheme,
        Action<AeroAuthenticationOptions> configuration)
    {
        return builder.AddAero(scheme, AeroAuthenticationDefaults.DisplayName, configuration);
    }

    /// <summary>
    /// Adds <see cref="AeroAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables Aero.VI authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="scheme">The authentication scheme associated with this instance.</param>
    /// <param name="caption">The display name associated with this instance.</param>
    /// <param name="configuration">The delegate used to configure the Aero.VI options.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static AuthenticationBuilder AddAero(
        this AuthenticationBuilder builder,
        string scheme,
        string caption,
        Action<AeroAuthenticationOptions> configuration)
    {
        return builder.AddOAuth<AeroAuthenticationOptions, AeroAuthenticationHandler>(scheme, caption, configuration);
    }
}
