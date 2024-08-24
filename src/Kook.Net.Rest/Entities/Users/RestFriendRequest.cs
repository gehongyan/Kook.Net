using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
    internal RestFriendRequest(BaseKookClient kook, Model model) :
        base(kook, model.Id)
    {
        Update(model);
    }

    internal static RestFriendRequest Create(BaseKookClient kook, Model model) => new(kook, model);

    [MemberNotNull(nameof(User))]
    internal void Update(Model model)
    {
        User = RestUser.Create(Kook, model.User);
    }

    private string DebuggerDisplay =>
        $"{User.UsernameAndIdentifyNumber(Kook.FormatUsersInBidirectionalUnicode)} ({Id}{
            (User.IsBot ?? false ? ", Bot" : "")}{(User.IsSystemUser ? ", System" : "")})";

    /// <inheritdoc />
    public Task AcceptAsync(RequestOptions? options = null) =>
        UserHelper.HandleFriendRequestAsync(this, true, Kook, options);

    /// <inheritdoc />
    public Task DeclineAsync(RequestOptions? options = null) =>
        UserHelper.HandleFriendRequestAsync(this, false, Kook, options);
}
