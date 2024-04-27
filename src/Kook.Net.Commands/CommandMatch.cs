namespace Kook.Commands;

/// <summary>
///     Represents a matched command.
/// </summary>
public struct CommandMatch
{
    /// <summary> The command that matches the search result. </summary>
    public CommandInfo Command { get; }

    /// <summary> The alias of the command. </summary>
    public string Alias { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CommandMatch"/> struct.
    /// </summary>
    /// <param name="command"> The command that matches the search result. </param>
    /// <param name="alias"> The alias of the command. </param>
    public CommandMatch(CommandInfo command, string alias)
    {
        Command = command;
        Alias = alias;
    }

    /// <summary>
    ///     Checks the preconditions of this command.
    /// </summary>
    /// <param name="context"> The context of the command. </param>
    /// <param name="services"> The services to use. </param>
    /// <returns> The result of the precondition check. </returns>
    public Task<PreconditionResult> CheckPreconditionsAsync(ICommandContext context,
        IServiceProvider? services = null) =>
        Command.CheckPreconditionsAsync(context, services);

    /// <summary>
    ///     Parses this command.
    /// </summary>
    /// <param name="context"> The context of the command. </param>
    /// <param name="searchResult"> The search result of the command. </param>
    /// <param name="preconditionResult"> The result of the precondition check. </param>
    /// <param name="services"> The services to use. </param>
    /// <returns> The result of the parse. </returns>
    public Task<ParseResult> ParseAsync(ICommandContext context, SearchResult searchResult,
        PreconditionResult? preconditionResult = null, IServiceProvider? services = null) =>
        Command.ParseAsync(context, Alias.Length, searchResult, preconditionResult, services);

    /// <summary>
    ///     Executes this command.
    /// </summary>
    /// <param name="context"> The context of the command. </param>
    /// <param name="argList"> The arguments of the command. </param>
    /// <param name="paramList"> The parameters of the command. </param>
    /// <param name="services"> The services to use. </param>
    /// <returns> The result of the execution. </returns>
    public Task<IResult> ExecuteAsync(ICommandContext context, IEnumerable<object> argList,
        IEnumerable<object> paramList, IServiceProvider services) =>
        Command.ExecuteAsync(context, argList, paramList, services);

    /// <summary>
    ///     Executes this command.
    /// </summary>
    /// <param name="context"> The context of the command. </param>
    /// <param name="parseResult"> The result of the parse. </param>
    /// <param name="services"> The services to use. </param>
    /// <returns> The result of the execution. </returns>
    public Task<IResult> ExecuteAsync(ICommandContext context, ParseResult parseResult, IServiceProvider services) =>
        Command.ExecuteAsync(context, parseResult, services);
}
