namespace Kook;

/// <summary>
///     表示网关事件意图的枚举，用于描述网关对 Bot 连接下发的事件类型。
/// </summary>
[Flags]
public enum GatewayIntents : byte
{
    /// <summary>
    ///     无。
    /// </summary>
    None = 0,

    /// <summary>
    ///     用户相关事件。
    /// </summary>
    /// <remarks>
    ///     包括用户加入服务器、加入频道、用户退出服务器、退出频道、点击按钮、用户信息更新。
    /// </remarks>
    Users = 1 << 0,

    /// <summary>
    ///     服务器相关事件。
    /// </summary>
    /// <remarks>
    ///     包括更新服务器、解散服务器、拉黑用户、取消拉黑。
    /// </remarks>
    Guilds = 1 << 1,

    /// <summary>
    ///     服务器角色相关事件。
    /// </summary>
    /// <remarks>
    ///     包括添加角色、删除角色、修改角色。
    /// </remarks>
    GuildRoles = 1 << 2,

    /// <summary>
    ///     频道相关事件。
    /// </summary>
    /// <remarks>
    ///     包括新增频道、删除频道、修改频道。
    /// </remarks>
    GuildChannels = 1 << 3,

    /// <summary>
    ///     频道消息相关事件。
    /// </summary>
    /// <remarks>
    ///     包括添加回应、删除回应、更新消息、删除消息、置顶消息、取消置顶消息。
    /// </remarks>
    GuildMessages = 1 << 4,

    /// <summary>
    ///     频道成员相关事件。
    /// </summary>
    /// <remarks>
    ///     包括加入服务器、加入频道、退出服务器、成员离线、成员下线、成员信息更新。
    /// </remarks>
    GuildMembers = 1 << 5,

    /// <summary>
    ///     表情相关事件。
    /// </summary>
    /// <remarks>
    ///     包括添加表情、删除表情、修改表情。
    /// </remarks>
    GuildEmojis = 1 << 6,

    /// <summary>
    ///     私聊消息相关事件。
    /// </summary>
    /// <remarks>
    ///     添加私聊回应、删除私聊回应、更新私聊消息、删除私聊消息
    /// </remarks>
    DirectMessages = 1 << 7,

    /// <summary>
    ///     表示所有事件意图。
    /// </summary>
    All = Users | Guilds | GuildRoles | GuildChannels | GuildMessages | GuildMembers | GuildEmojis | DirectMessages
}
