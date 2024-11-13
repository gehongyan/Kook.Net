namespace Kook;

/// <summary>
///     表示客户端所使用的令牌类型。
/// </summary>
public enum TokenType
{
    /// <summary>
    ///     OAuth2 令牌类型。
    /// </summary>
    Bearer,

    /// <summary>
    ///     Bot 令牌类型。
    /// </summary>
    Bot,

    /// <summary>
    ///     管道令牌类型。
    /// </summary>
    Pipe,
}
