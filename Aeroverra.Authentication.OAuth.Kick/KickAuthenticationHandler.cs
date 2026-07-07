using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aeroverra.Authentication.OAuth.Kick;

/// <summary>
/// Provides the OAuth 2.0 authentication logic for Kick.
/// </summary>
/// <param name="options">The monitor for the options instance.</param>
/// <param name="logger">The logger factory to use.</param>
/// <param name="encoder">The URL encoder to use.</param>
public partial class KickAuthenticationHandler(
    IOptionsMonitor<KickAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : OAuthHandler<KickAuthenticationOptions>(options, logger, encoder)
{
    /// <inheritdoc />
    protected override async Task<AuthenticationTicket> CreateTicketAsync(
        ClaimsIdentity identity,
        AuthenticationProperties properties,
        OAuthTokenResponse tokens)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        using var response = await Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, Context.RequestAborted);

        if (!response.IsSuccessStatusCode)
        {
            await Log.UserProfileErrorAsync(Logger, response, Context.RequestAborted);
            throw new HttpRequestException("An error occurred while retrieving the user profile from Kick.");
        }

        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync(Context.RequestAborted));

        // The users endpoint wraps its result in a "data" array containing the authorized user.
        if (!payload.RootElement.TryGetProperty("data", out var data) ||
            data.ValueKind != JsonValueKind.Array ||
            data.GetArrayLength() < 1)
        {
            throw new InvalidOperationException("The user profile returned by Kick does not contain any user data.");
        }

        var principal = new ClaimsPrincipal(identity);
        var context = new OAuthCreatingTicketContext(
            principal,
            properties,
            Context,
            Scheme,
            Options,
            Backchannel,
            tokens,
            data[0]);
        context.RunClaimActions();

        await Events.CreatingTicket(context);
        return new AuthenticationTicket(context.Principal!, context.Properties, Scheme.Name);
    }

    private static partial class Log
    {
        internal static async Task UserProfileErrorAsync(ILogger logger, HttpResponseMessage response, CancellationToken cancellationToken)
        {
            UserProfileError(
                logger,
                response.StatusCode,
                response.Headers.ToString(),
                await response.Content.ReadAsStringAsync(cancellationToken));
        }

        [LoggerMessage(1,
            LogLevel.Error,
            "An error occurred while retrieving the user profile: the remote server returned a {Status} response with the following payload: {Headers} {Body}.")]
        private static partial void UserProfileError(
            ILogger logger,
            System.Net.HttpStatusCode status,
            string headers,
            string body);
    }
}
