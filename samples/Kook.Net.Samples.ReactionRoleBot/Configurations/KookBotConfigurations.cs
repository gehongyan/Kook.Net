using Kook.Net.Samples.ReactionRoleBot.Enums;

namespace Kook.Net.Samples.ReactionRoleBot.Configurations;

public class KookBotConfigurations
{
    /// <summary>
    ///     Token
    /// </summary>
    public string Token { get; init; }

    /// <summary>
    ///     频道ID列表
    /// </summary>
    public KookChannels Channels { get; init; }

    /// <summary>
    ///     角色ID列表
    /// </summary>
    public KookRoles Roles { get; init; }
    
    /// <summary>
    ///     KOOK Bot运行环境
    /// </summary>
    public BotEnvironment KookBotEnvironment { get; init; }
}

public class KookChannels
{
    public ulong ReactionChannelId { get; init; }
}

public class KookRoles
{
    public ulong DeveloperRoleId { get; init; }
}