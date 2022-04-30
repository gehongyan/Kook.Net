using System.ComponentModel;

namespace KaiHeiLa.Net.Samples.ReactionRoleBot.Enums;

public enum BotEnvironment
{
    /// <summary>
    ///     调试
    /// </summary>
    [Description("调试")]
    Development,

    /// <summary>
    ///     生产
    /// </summary>
    [Description("生产")]
    Production,
}