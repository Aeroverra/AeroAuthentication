using Aeroverra.Authentication.AeroVi;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to add Aero.VI authentication capabilities to an HTTP application pipeline.
/// </summary>
public static class AeroViAuthenticationExtensions
{
    /// <summary>
    /// Adds <see cref="AeroViAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables Aero.VI authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static AuthenticationBuilder AddAeroVi(this AuthenticationBuilder builder)
    {
        return builder.AddAeroVi(AeroViAuthenticationDefaults.AuthenticationScheme, static options => { });
    }

    /// <summary>
    /// Adds <see cref="AeroViAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables Aero.VI authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="configuration">The delegate used to configure the Aero.VI options.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static AuthenticationBuilder AddAeroVi(
        this AuthenticationBuilder builder,
        Action<AeroViAuthenticationOptions> configuration)
    {
        return builder.AddAeroVi(AeroViAuthenticationDefaults.AuthenticationScheme, configuration);
    }

    /// <summary>
    /// Adds <see cref="AeroViAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables Aero.VI authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="scheme">The authentication scheme associated with this instance.</param>
    /// <param name="configuration">The delegate used to configure the Aero.VI options.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static AuthenticationBuilder AddAeroVi(
        this AuthenticationBuilder builder,
        string scheme,
        Action<AeroViAuthenticationOptions> configuration)
    {
        return builder.AddAeroVi(scheme, AeroViAuthenticationDefaults.DisplayName, configuration);
    }

    /// <summary>
    /// Adds <see cref="AeroViAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables Aero.VI authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="scheme">The authentication scheme associated with this instance.</param>
    /// <param name="caption">The display name associated with this instance.</param>
    /// <param name="configuration">The delegate used to configure the Aero.VI options.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static AuthenticationBuilder AddAeroVi(
        this AuthenticationBuilder builder,
        string scheme,
        string caption,
        Action<AeroViAuthenticationOptions> configuration)
    {
        return builder.AddOAuth<AeroViAuthenticationOptions, AeroViAuthenticationHandler>(scheme, caption, configuration);
    }
}
