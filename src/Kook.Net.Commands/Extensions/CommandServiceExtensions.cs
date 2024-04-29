namespace Kook.Commands;

/// <summary>
///     Provides extension methods for the <see cref="CommandService"/> class.
/// </summary>
public static class CommandServiceExtensions
{
    /// <summary>
    ///     Returns commands that can be executed under the current context.
    /// </summary>
    /// <param name="commands">The set of commands to be checked against.</param>
    /// <param name="context">The current command context.</param>
    /// <param name="provider">The service provider used for dependency injection upon precondition check.</param>
    /// <returns>
    ///     A read-only collection of commands that can be executed under the current context.
    /// </returns>
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
    ///     Returns commands that can be executed under the current context.
    /// </summary>
    /// <param name="commandService">The desired command service class to check against.</param>
    /// <param name="context">The current command context.</param>
    /// <param name="provider">The service provider used for dependency injection upon precondition check.</param>
    /// <returns>
    ///     A read-only collection of commands that can be executed under the current context.
    /// </returns>
    public static Task<IReadOnlyCollection<CommandInfo>> GetExecutableCommandsAsync(this CommandService commandService,
        ICommandContext context, IServiceProvider provider) =>
        GetExecutableCommandsAsync(commandService.Commands.ToArray(), context, provider);

    /// <summary>
    ///     Returns commands that can be executed under the current context.
    /// </summary>
    /// <param name="module">The module to be checked against.</param>
    /// <param name="context">The current command context.</param>
    /// <param name="provider">The service provider used for dependency injection upon precondition check.</param>
    /// <returns>
    ///     A read-only collection of commands that can be executed under the current context.
    /// </returns>
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
