namespace Kook;

/// <summary>
///     表示一个通用的用户之间的亲密关系请求。
/// </summary>
public interface IIntimacyRelationRequest : IFriendRequest
{
    /// <summary>
    ///     获取此亲密关系的关系类型。
    /// </summary>
    IntimacyRelationType? IntimacyType { get; }
}
