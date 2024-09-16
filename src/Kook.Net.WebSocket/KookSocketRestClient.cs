using Kook.Rest;

namespace Kook.WebSocket;

/// <summary>
///     表示一个用于网关客户端内的 REST 客户端。
/// </summary>
public class KookSocketRestClient : KookRestClient
{
    internal KookSocketRestClient(KookRestConfig config, API.KookRestApiClient api) : base(config, api)
    {
    }

    /// <inheritdoc cref="Kook.Rest.BaseKookClient.LoginAsync(Kook.TokenType,System.String,System.Boolean)" />
    /// <exception cref="NotSupportedException"> 网关客户端内的 REST 客户端无法进行登录或退出登录。 </exception>
    public new Task LoginAsync(TokenType tokenType, string token, bool validateToken = true) =>
        throw new NotSupportedException("The Socket REST wrapper cannot be used to log in or out.");

    internal override Task LoginInternalAsync(TokenType tokenType, string token, bool validateToken) =>
        throw new NotSupportedException("The Socket REST wrapper cannot be used to log in or out.");

    /// <inheritdoc cref="Kook.Rest.BaseKookClient.LogoutAsync" />
    /// <exception cref="NotSupportedException"> 网关客户端内的 REST 客户端无法进行登录或退出登录。 </exception>
    public new Task LogoutAsync() =>
        throw new NotSupportedException("The Socket REST wrapper cannot be used to log in or out.");

    internal override Task LogoutInternalAsync() =>
        throw new NotSupportedException("The Socket REST wrapper cannot be used to log in or out.");
}
