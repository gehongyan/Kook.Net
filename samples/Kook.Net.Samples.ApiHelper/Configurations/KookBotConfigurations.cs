using Kook.Net.Samples.ApiHelper.Enums;

namespace Kook.Net.Samples.ApiHelper.Configurations;

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
    
}

public class KookRoles
{
    
}