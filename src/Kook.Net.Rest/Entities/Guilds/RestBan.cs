using System.Diagnostics;
using Model = Kook.API.Ban;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的封禁对象。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestBan : IBan
{
    #region RestBan

    /// <summary>
    ///     获取被封禁的用户。
    /// </summary>
    public RestUser User { get; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; }

    /// <inheritdoc />
    public string Reason { get; }

    internal RestBan(RestUser user, string reason, DateTimeOffset createdAt)
    {
        User = user;
        Reason = reason;
        CreatedAt = createdAt;
    }

    internal static RestBan Create(BaseKookClient client, Model model) =>
        new(RestUser.Create(client, model.User), model.Reason, model.CreatedAt);

    /// <summary>
    ///     获取被封禁用户的包含用户名及识别号的格式化字符串。
    /// </summary>
    /// <returns> 被封禁用户的包含用户名及识别号的格式化字符串。 </returns>
    /// <seealso cref="Kook.Rest.RestBan.User"/>
    /// <seealso cref="Kook.Format.UsernameAndIdentifyNumber(Kook.IUser,System.Boolean)"/>
    public override string ToString() => User.ToString();

    private string DebuggerDisplay => $"{User}: {Reason}";

    #endregion

    #region IBan

    /// <inheritdoc />
    IUser IBan.User => User;

    #endregion
}
