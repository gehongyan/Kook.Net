namespace Kook.Commands;

/// <summary>
///     提供用于 <see cref="T:Kook.Commands.CommandService"/> 的扩展方法。
/// </summary>
public static class CommandServiceExtensions
{
    /// <summary>
    ///     获取可以在当前上下文下执行的命令。
    /// </summary>
    /// <param name="commands"> 要检查的命令。 </param>
    /// <param name="context"> 当前命令上下文。 </param>
    /// <param name="provider"> 用于检查时的依赖注入的服务提供程序。 </param>
    /// <returns> 所提供的命令中可以在当前上下文下执行的所有命令。 </returns>
    public static async Task<IReadOnlyCollection<CommandInfo>> GetExecutableCommandsAsync(this ICollection<CommandInfo> commands,
        ICommandContext context, IServiceProvider provider)
    {
        List<CommandInfo> executableCommands = [];
        var tasks = commands.Select(async c =>
        {
            PreconditionResult result = await c.CheckPreconditionsAsync(context, provider).ConfigureAwait(false);
            return new { Command = c, PreconditionResult = result };
        });

        var results = await Task.WhenAll(tasks).ConfigureAwait(false);
        IEnumerable<CommandInfo> successes = results
            .Where(r => r.PreconditionResult.IsSuccess)
            .Select(r => r.Command);
        executableCommands.AddRange(successes);
        return executableCommands;
    }

    /// <summary>
    ///     获取可以在当前上下文下执行的命令。
    /// </summary>
    /// <param name="commandService"> 要检查的命令服务。 </param>
    /// <param name="context"> 当前命令上下文。 </param>
    /// <param name="provider"> 用于检查时的依赖注入的服务提供程序。 </param>
    /// <returns> 可以在当前上下文下执行的所有命令。 </returns>
    public static Task<IReadOnlyCollection<CommandInfo>> GetExecutableCommandsAsync(this CommandService commandService,
        ICommandContext context, IServiceProvider provider) =>
        GetExecutableCommandsAsync(commandService.Commands.ToArray(), context, provider);

    /// <summary>
    ///     获取可以在当前上下文下执行的命令。
    /// </summary>
    /// <param name="module"> 要检查的模块。 </param>
    /// <param name="context"> 当前命令上下文。 </param>
    /// <param name="provider"> 用于检查时的依赖注入的服务提供程序。 </param>
    /// <returns> 所提供的模块中可以在当前上下文下执行的所有命令。 </returns>
    public static async Task<IReadOnlyCollection<CommandInfo>> GetExecutableCommandsAsync(this ModuleInfo module, ICommandContext context,
        IServiceProvider provider)
    {
        List<CommandInfo> executableCommands = [];

        executableCommands.AddRange(await module.Commands.ToArray().GetExecutableCommandsAsync(context, provider).ConfigureAwait(false));
        IEnumerable<Task<IReadOnlyCollection<CommandInfo>>> tasks = module
            .Submodules
            .Select(async s => await s.GetExecutableCommandsAsync(context, provider).ConfigureAwait(false));
        IReadOnlyCollection<CommandInfo>[] results = await Task.WhenAll(tasks).ConfigureAwait(false);
        executableCommands.AddRange(results.SelectMany(c => c));
        return executableCommands;
    }
}
