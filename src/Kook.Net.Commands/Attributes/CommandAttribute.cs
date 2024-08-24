namespace Kook.Commands;

/// <summary>
///     为命令标记执行信息。
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    /// <summary>
    ///     获取此命令的基本名称。
    /// </summary>
    /// <remarks>
    ///     <see cref="T:Kook.Commands.NameAttribute"/> 特性重写此属性的值。
    /// </remarks>
    public string? Text { get; }

    /// <summary>
    ///     获取或设置命令的运行模式。
    /// </summary>
    /// <seealso cref="P:Kook.Commands.CommandServiceConfig.DefaultRunMode"/>
    public RunMode RunMode { get; set; } = RunMode.Default;

    /// <summary>
    ///     获取或设置是否忽略此命令的额外参数。
    /// </summary>
    /// <seealso cref="P:Kook.Commands.CommandServiceConfig.IgnoreExtraArgs"/>
    public bool? IgnoreExtraArgs { get; }

    /// <summary>
    ///     获取或设置命令的摘要。
    /// </summary>
    /// <remarks>
    ///     <see cref="T:Kook.Commands.SummaryAttribute"/> 特性重写此属性的值。
    /// </remarks>
    public string? Summary { get; set; }

    /// <summary>
    ///     获取或设置命令的别名。
    /// </summary>
    /// <remarks>
    ///     <see cref="T:Kook.Commands.AliasAttribute"/> 特性的值会与此属性的值合并。
    /// </remarks>
    public string?[]? Aliases { get; set; }

    /// <summary>
    ///     获取或设置命令的备注。
    /// </summary>
    /// <remarks>
    ///     <see cref="T:Kook.Commands.RemainderAttribute"/> 特性重写此属性的值。
    /// </remarks>
    public string? Remarks { get; set; }

    /// <summary>
    ///     初始化一个 <see cref="CommandAttribute"/> 类的新实例。
    /// </summary>
    public CommandAttribute()
    {
    }

    /// <summary>
    ///     初始化一个 <see cref="CommandAttribute"/> 类的新实例。
    /// </summary>
    /// <param name="text"> 命令的基本名称。 </param>
    public CommandAttribute(string text)
    {
        Text = text;
    }

    /// <summary>
    ///     初始化一个 <see cref="CommandAttribute"/> 类的新实例。
    /// </summary>
    /// <param name="text"> 命令的基本名称。 </param>
    /// <param name="ignoreExtraArgs"> 是否忽略此命令的额外参数。 </param>
    /// <param name="summary"> 命令的摘要。 </param>
    /// <param name="aliases"> 命令的别名。 </param>
    /// <param name="remarks"> 命令的备注。 </param>
    public CommandAttribute(string text, bool ignoreExtraArgs,
        string? summary = null, string?[]? aliases = null, string? remarks = null)
    {
        Text = text;
        IgnoreExtraArgs = ignoreExtraArgs;
        Summary = summary;
        Aliases = aliases;
        Remarks = remarks;
    }
}
