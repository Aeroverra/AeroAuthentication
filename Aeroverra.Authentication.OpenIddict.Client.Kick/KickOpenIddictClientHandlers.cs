using System.Text.Json;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Client.OpenIddictClientEvents;

namespace Aeroverra.Authentication.OpenIddict.Client.Kick;

/// <summary>
/// Contains the OpenIddict client event handlers used by the Kick provider.
/// </summary>
public static class KickOpenIddictClientHandlers
{
    /// <summary>
    /// Contains the logic responsible for normalizing the userinfo response returned by Kick:
    /// the payload is unwrapped from its "data" array and the non-standard parameters are
    /// mapped to their standard OpenID Connect equivalents.
    /// </summary>
    public sealed class NormalizeUserInfoResponse : IOpenIddictClientHandler<ExtractUserInfoResponseContext>
    {
        /// <summary>
        /// Gets the default descriptor definition assigned to this handler.
        /// </summary>
        public static OpenIddictClientHandlerDescriptor Descriptor { get; }
            = OpenIddictClientHandlerDescriptor.CreateBuilder<ExtractUserInfoResponseContext>()
                .UseSingletonHandler<NormalizeUserInfoResponse>()
                .SetOrder(int.MaxValue - 50_000)
                .SetType(OpenIddictClientHandlerType.Custom)
                .Build();

        /// <inheritdoc />
        public ValueTask HandleAsync(ExtractUserInfoResponseContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (!string.Equals(context.Registration.ProviderName, KickOpenIddictClientDefaults.ProviderName, StringComparison.Ordinal))
            {
                return ValueTask.CompletedTask;
            }

            // The users endpoint wraps the authorized user in a "data" array.
            var user = (JsonElement?)context.Response?["data"]?[0] ??
                throw new InvalidOperationException("The userinfo response returned by Kick does not contain any user data.");

            if (user.ValueKind is not JsonValueKind.Object)
            {
                throw new InvalidOperationException("The userinfo response returned by Kick does not contain any user data.");
            }

            var response = new OpenIddictResponse(user);

            // Kick returns the user identifier as a "user_id" integer and the profile picture
            // as "profile_picture"; map them to their standard OpenID Connect equivalents.
            response[Claims.Subject] = (string?)response["user_id"];
            response[Claims.Picture] = (string?)response["profile_picture"];

            context.Response = response;

            return ValueTask.CompletedTask;
        }
    }
}
