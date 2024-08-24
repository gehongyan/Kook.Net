namespace Kook.Commands;

/// <summary>
///     为命令指定别名。
/// </summary>
/// <remarks>
///     此特性允许命令具有一个或多个别名，在指定命令的基本名称的同时，还可以指定多个别名，以便用户可以使用多个熟悉的词汇来触发相同的命令。
/// </remarks>
/// <example>
///     以下示例中，要调用此命令，除了可以使用基本名称“stats”，还使用“stat”或“info”。
///     <code language="cs">
///     [Command("stats")]
///     [Alias("stat", "info")]
///     public async Task GetStatsAsync(IUser user)
///     {
///         await ReplyTextAsync($"{user.Username} has 1000 score!");
///     }
///     </code>
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AliasAttribute : Attribute
{
    /// <summary>
    ///     获取为命令定义的别名。
    /// </summary>
    public string[] Aliases { get; }

    /// <summary>
    ///     初始化一个 <see cref="AliasAttribute"/> 类的新实例。
    /// </summary>
    public AliasAttribute(params string[] aliases)
    {
        Aliases = aliases;
    }
}
