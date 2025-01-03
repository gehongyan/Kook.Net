namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的亲密关系请求。
/// </summary>
public class RestIntimacyRelationRequest : RestFriendRequest, IIntimacyRelationRequest
{
    internal RestIntimacyRelationRequest(BaseKookClient kook, ulong id)
        : base(kook, id)
    { }

    /// <inheritdoc />
    public IntimacyRelationType? IntimacyType { get; internal set; }

    internal static new RestIntimacyRelationRequest Create(BaseKookClient kook, API.Rest.FriendState model)
    {
        RestIntimacyRelationRequest entity = new(kook, model.Id);
        entity.Update(model);
        return entity;
    }

    internal override void Update(API.Rest.FriendState model)
    {
        base.Update(model);
        IntimacyType = model.IntimacyType;
    }

    /// <inheritdoc />
    public override Task AcceptAsync(RequestOptions? options = null) =>
        UserHelper.HandleIntimacyRequestAsync(this, true, Kook, options);

    /// <inheritdoc />
    public override Task DeclineAsync(RequestOptions? options = null) =>
        UserHelper.HandleIntimacyRequestAsync(this, false, Kook, options);
}
