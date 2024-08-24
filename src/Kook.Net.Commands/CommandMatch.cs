namespace Kook.Commands;

/// <summary>
///     表示一个匹配的命令。
/// </summary>
public struct CommandMatch
{
    /// <summary>
    ///     获取与搜索结果相匹配的命令。
    /// </summary>
    public CommandInfo Command { get; }

    /// <summary>
    ///     获取命令的别名。
    /// </summary>
    public string Alias { get; }

    /// <summary>
    ///     初始化一个 <see cref="CommandMatch" /> 结构的新实例。
    /// </summary>
    /// <param name="command"> 与搜索结果相匹配的命令。 </param>
    /// <param name="alias"> 命令的别名。 </param>
    public CommandMatch(CommandInfo command, string alias)
    {
        Command = command;
        Alias = alias;
    }

    /// <summary>
    ///     检查此命令的先决条件。
    /// </summary>
    /// <param name="context"> 命令的上下文。 </param>
    /// <param name="services"> 用于检查先决条件的以来服务提供程序。 </param>
    /// <returns> 一个表示异步检查操作的任务。任务的结果包含先决条件的结果。 </returns>
    public Task<PreconditionResult> CheckPreconditionsAsync(ICommandContext context,
        IServiceProvider? services = null) =>
        Command.CheckPreconditionsAsync(context, services);

    /// <summary>
    ///     解析此命令。
    /// </summary>
    /// <param name="context"> 命令的上下文。 </param>
    /// <param name="searchResult"> 搜索结果。 </param>
    /// <param name="preconditionResult"> 先决条件的结果。 </param>
    /// <param name="services"> 用于解析的服务提供程序。 </param>
    /// <returns> 一个表示异步解析操作的任务。任务的结果包含解析的结果。 </returns>
    public Task<ParseResult> ParseAsync(ICommandContext context, SearchResult searchResult,
        PreconditionResult? preconditionResult = null, IServiceProvider? services = null) =>
        Command.ParseAsync(context, Alias.Length, searchResult, preconditionResult, services);

    /// <summary>
    ///     执行此命令。
    /// </summary>
    /// <param name="context"> 命令的上下文。 </param>
    /// <param name="argList"> 命令的实参。 </param>
    /// <param name="paramList"> 命令的形参。 </param>
    /// <param name="services"> 用于执行的服务提供程序。 </param>
    /// <returns> 一个表示异步执行操作的任务。任务的结果包含执行的结果。 </returns>
    public Task<IResult> ExecuteAsync(ICommandContext context, IEnumerable<object> argList,
        IEnumerable<object> paramList, IServiceProvider services) =>
        Command.ExecuteAsync(context, argList, paramList, services);

    /// <summary>
    ///     执行此命令。
    /// </summary>
    /// <param name="context"> 命令的上下文。 </param>
    /// <param name="parseResult"> 解析的结果。 </param>
    /// <param name="services"> 用于执行的服务提供程序。 </param>
    /// <returns> 一个表示异步执行操作的任务。任务的结果包含执行的结果。 </returns>
    public Task<IResult> ExecuteAsync(ICommandContext context, ParseResult parseResult, IServiceProvider services) =>
        Command.ExecuteAsync(context, parseResult, services);
}
