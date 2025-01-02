using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的亲密关系。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestIntimacyRelation : RestEntity<ulong>, IIntimacyRelation
{
    /// <inheritdoc />
    public IUser User { get; internal set; }

    /// <inheritdoc />
    public IntimacyRelationType RelationType { get; internal set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; internal set; }

    /// <inheritdoc />
    internal RestIntimacyRelation(BaseKookClient kook, API.Rest.FriendState model)
        : base(kook, model.Id)
    {
        Update(model);
    }

    internal static RestIntimacyRelation Create(BaseKookClient kook, API.Rest.FriendState model) => new(kook, model);

    [MemberNotNull(nameof(User))]
    internal void Update(API.Rest.FriendState model)
    {
        User = RestUser.Create(Kook, model.User);
        if (!model.IntimacyType.HasValue)
            throw new InvalidOperationException("Intimacy type is not provided.");
        RelationType = model.IntimacyType.Value;
        if (!model.RelationTime.HasValue)
            throw new InvalidOperationException("Relation time is not provided.");
        CreatedAt = model.RelationTime.Value;
    }

    private string DebuggerDisplay =>
        $"{User.UsernameAndIdentifyNumber(Kook.FormatUsersInBidirectionalUnicode)} ({Id}{
            (User.IsBot ?? false ? ", Bot" : "")}{(User.IsSystemUser ? ", System" : "")}, {RelationType})";

    /// <inheritdoc />
    public Task UnravelAsync(bool removeFriend,  RequestOptions? options = null) =>
        User.UnravelIntimacyRelationAsync(removeFriend, options);
}
