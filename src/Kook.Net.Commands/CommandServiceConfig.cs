namespace Kook.Commands;

/// <summary>
///     表示一个用于 <see cref="Kook.Commands.CommandService"/> 的配置类。
/// </summary>
public class CommandServiceConfig
{
    /// <summary>
    ///     获取或设置命令的默认运行模式，如果在命令属性或构建器上未指定运行模式，则使用此值。
    /// </summary>
    /// <remarks>
    ///     如需在命令上重写默认行为，请使用 <see cref="Kook.Commands.CommandAttribute.RunMode"/> 属性。
    /// </remarks>
    public RunMode DefaultRunMode { get; set; } = RunMode.Sync;

    /// <summary>
    ///     获取或设置用于分隔参数的字符。
    /// </summary>
    public char SeparatorChar { get; set; } = ' ';

    /// <summary>
    ///     获取或设置命令是否区分大小写。
    /// </summary>
    public bool CaseSensitiveCommands { get; set; } = false;

    /// <summary>
    ///     获取或设置将引发 <see cref="Kook.Commands.CommandService.Log"/> 事件的最低日志级别严重性。
    /// </summary>
    public LogSeverity LogLevel { get; set; } = LogSeverity.Info;

    /// <summary>
    ///     获取或设置同步执行的命令是否应将异常传递给调用者。
    /// </summary>
    /// <remarks>
    ///     同步命令指的是其运行模式为 <see cref="Kook.Commands.RunMode.Sync"/> 的命令。
    /// </remarks>
    public bool ThrowOnError { get; set; } = true;

    /// <summary>
    ///     获取或设置用于匹配字符串定界符的别名集合。
    /// </summary>
    /// <remarks>
    ///     如果为更改此属性，则将使用默认的内置别名集合，这可能包含了许多地区和 Unicode
    ///     符号中可视为开闭对的符号。也可以在此默认集合的基础上进行修改。如果设置为空字典，则会使用 <c>"</c> 作为默认定界符。
    /// </remarks>
    /// <example>
    ///     以下示例重新设置了定界符所使用的开闭对：
    ///     <code language="cs">
    ///         QuotationMarkAliasMap = new Dictionary&lt;char, char&gt;()
    ///         {
    ///             {'\"', '\"' },
    ///             {'“', '”' },
    ///             {'「', '」' },
    ///         }
    ///     </code>
    /// </example>
    public Dictionary<char, char> QuotationMarkAliasMap { get; set; } = QuotationAliasUtils.DefaultAliasMap;

    /// <summary>
    ///     获取或设置默认情况下是否应忽略额外的参数。
    /// </summary>
    /// <remarks>
    ///     如果设置为 <see langword="true"/>，则默认情况下命令将忽略额外的参数。否则，将引发
    ///     <see cref="Kook.Commands.CommandError.BadArgCount"/>
    ///     错误。如需在命令上重写默认行为，请使用 <see cref="Kook.Commands.CommandAttribute.IgnoreExtraArgs"/> 属性。
    /// </remarks>
    public bool IgnoreExtraArgs { get; set; } = false;
}
