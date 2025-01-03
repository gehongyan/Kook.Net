using System.Diagnostics;
using Model = Kook.API.Rest.FriendState;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的好友请求。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestFriendRequest : RestEntity<ulong>, IFriendRequest
{
    /// <inheritdoc />
    public IUser User { get; internal set; }

    /// <inheritdoc />
    internal RestFriendRequest(BaseKookClient kook, ulong id)
        : base(kook, id)
    {
        User = null!;
    }

    internal static RestFriendRequest Create(BaseKookClient kook, Model model)
    {
        RestFriendRequest entity = new(kook, model.Id);
        entity.Update(model);
        return entity;
    }

    internal virtual void Update(Model model)
    {
        User = RestUser.Create(Kook, model.User);
    }

    private string DebuggerDisplay =>
        $"{User.UsernameAndIdentifyNumber(Kook.FormatUsersInBidirectionalUnicode)} ({Id}{
            (User.IsBot ?? false ? ", Bot" : "")}{(User.IsSystemUser ? ", System" : "")})";

    /// <inheritdoc />
    public virtual Task AcceptAsync(RequestOptions? options = null) =>
        UserHelper.HandleFriendRequestAsync(this, true, Kook, options);

    /// <inheritdoc />
    public virtual Task DeclineAsync(RequestOptions? options = null) =>
        UserHelper.HandleFriendRequestAsync(this, false, Kook, options);
}
