namespace Kook;

/// <summary>
///     表示一个通用的用户之间的亲密关系。
/// </summary>
public interface IIntimacyRelation : IEntity<ulong>
{
    /// <summary>
    ///     获取发送此好友请求的用户。
    /// </summary>
    IUser User { get; }

    /// <summary>
    ///     获取此亲密关系的关系类型。
    /// </summary>
    IntimacyRelationType RelationType { get; }

    /// <summary>
    ///     获取此亲密关系的创建时间。
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    ///     解除此亲密关系。
    /// </summary>
    /// <param name="removeFriend"> 是否同时解除好友关系。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步解除操作的任务。 </returns>
    Task UnravelAsync(bool removeFriend = false, RequestOptions? options = null);
}
