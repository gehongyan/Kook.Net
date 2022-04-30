using KaiHeiLa.Net.Samples.ReactionRoleBot.Enums;

namespace KaiHeiLa.Net.Samples.ReactionRoleBot.Configurations;

public class KaiHeiLaBotConfigurations
{
    /// <summary>
    ///     Token
    /// </summary>
    public string Token { get; init; }

    /// <summary>
    ///     频道ID列表
    /// </summary>
    public KaiHeiLaChannels Channels { get; init; }

    /// <summary>
    ///     角色ID列表
    /// </summary>
    public KaiHeiLaRoles Roles { get; init; }
    
    /// <summary>
    ///     开黑啦Bot运行环境
    /// </summary>
    public BotEnvironment KaiHeiLaBotEnvironment { get; init; }
}

public class KaiHeiLaChannels
{
    public ulong ReactionChannelId { get; init; }
}

public class KaiHeiLaRoles
{
    public ulong DeveloperRoleId { get; init; }
}