namespace Kook;

/// <summary>
///     表示一个通用的邀请。
/// </summary>
public interface IInvite : IEntity<uint>, IDeletable
{
    /// <summary>
    ///     获取此邀请的唯一代码。
    /// </summary>
    string Code { get; }

    /// <summary>
    ///     获取用于接受此邀请的 URL，URL 的路径中包含 <see cref="Code"/> 属性的值。
    /// </summary>
    string Url { get; }

    /// <summary>
    ///     获取创建此邀请的用户。
    /// </summary>
    IUser Inviter { get; }

    /// <summary>
    ///     获取此邀请链接指向的频道。
    /// </summary>
    IChannel Channel { get; }

    /// <summary>
    ///     获取此邀请链接指向的频道的类型。
    /// </summary>
    ChannelType ChannelType { get; }

    /// <summary>
    ///     获取此邀请链接指向的频道的 ID。
    /// </summary>
    ulong? ChannelId { get; }

    /// <summary>
    ///     获取此邀请链接指向的频道的名称。
    /// </summary>
    string? ChannelName { get; }

    /// <summary>
    ///     获取此邀请链接指向的服务器。
    /// </summary>
    IGuild Guild { get; }

    /// <summary>
    ///     获取此邀请链接指向的服务器的 ID。
    /// </summary>
    ulong? GuildId { get; }

    /// <summary>
    ///     获取此邀请链接指向的服务器的名称。
    /// </summary>
    string GuildName { get; }

    /// <summary>
    ///     获取此邀请的创建时间。
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    ///     获取此邀请的过期时间。
    /// </summary>
    /// <remarks>
    ///     如果此邀请永不过期，则此属性的值为 <c>null</c>。
    /// </remarks>
    DateTimeOffset? ExpiresAt { get; }

    /// <summary>
    ///     获取此邀请的有效时长。
    /// </summary>
    /// <remarks>
    ///     如果此邀请永不过期，则此属性的值为 <c>null</c>。
    /// </remarks>
    TimeSpan? MaxAge { get; }

    /// <summary>
    ///     获取此邀请的可用人次。
    /// </summary>
    /// <remarks>
    ///     如果此邀请不限制可用人次，则此属性的值为 <c>null</c>。
    /// </remarks>
    int? MaxUses { get; }

    /// <summary>
    ///     获取此邀请已被使用的次数。
    /// </summary>
    int? Uses { get; }

    /// <summary>
    ///     获取此邀请剩余可用次数。
    /// </summary>
    /// <remarks>
    ///     如果此邀请不限制可用人次，则此属性的值为 <c>null</c>。
    /// </remarks>
    int? RemainingUses { get; }

    /// <summary>
    ///     获取已接受此邀请的用户数量。
    /// </summary>
    int InvitedUsersCount { get; }
}
