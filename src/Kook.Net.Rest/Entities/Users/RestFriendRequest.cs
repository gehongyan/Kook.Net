using System.Diagnostics;
using Model = Kook.API.Rest.FriendState;

namespace Kook.Rest;

/// <summary>
///     Represents a REST-based friend request.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestFriendRequest : RestEntity<ulong>, IFriendRequest
{
    /// <inheritdoc />
    internal RestFriendRequest(BaseKookClient kook, ulong id) : base(kook, id)
    {
    }

    internal static RestFriendRequest Create(BaseKookClient kook, Model model)
    {
        RestFriendRequest entity = new(kook, model.Id);
        entity.Update(model);
        return entity;
    }

    internal virtual void Update(Model model) => User = RestUser.Create(Kook, model.User);

    private string DebuggerDisplay =>
        $"{Format.UsernameAndIdentifyNumber(User, Kook.FormatUsersInBidirectionalUnicode)} ({Id}{(User.IsBot ?? false ? ", Bot" : "")})";

    /// <inheritdoc />
    public IUser User { get; internal set; }

    /// <inheritdoc />
    public Task AcceptAsync(RequestOptions? options = null) =>
        UserHelper.HandleFriendRequestAsync(this, true, Kook, options);

    /// <inheritdoc />
    public Task DeclineAsync(RequestOptions? options = null) =>
        UserHelper.HandleFriendRequestAsync(this, false, Kook, options);
}
