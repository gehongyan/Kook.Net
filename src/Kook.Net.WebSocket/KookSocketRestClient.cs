using Kook.Rest;

namespace Kook.WebSocket;

/// <summary>
///     Represents an REST-only client that is used in a WebSocket-based client.
/// </summary>
public class KookSocketRestClient : KookRestClient
{
    internal KookSocketRestClient(KookRestConfig config, API.KookRestApiClient api) : base(config, api)
    {
    }

    /// <summary>
    ///     Throws a <see cref="NotSupportedException"/> when trying to log in.
    /// </summary>
    /// <exception cref="NotSupportedException"> The Socket REST wrapper cannot be used to log in or out. </exception>
    public new Task LoginAsync(TokenType tokenType, string token, bool validateToken = true) =>
        throw new NotSupportedException("The Socket REST wrapper cannot be used to log in or out.");

    internal override Task LoginInternalAsync(TokenType tokenType, string token, bool validateToken) =>
        throw new NotSupportedException("The Socket REST wrapper cannot be used to log in or out.");

    /// <summary>
    ///     Throws a <see cref="NotSupportedException"/> when trying to log out.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"> The Socket REST wrapper cannot be used to log in or out. </exception>
    public new Task LogoutAsync() =>
        throw new NotSupportedException("The Socket REST wrapper cannot be used to log in or out.");

    internal override Task LogoutInternalAsync() =>
        throw new NotSupportedException("The Socket REST wrapper cannot be used to log in or out.");
}
