using Kook.Commands.Builders;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Kook.Commands;

/// <summary>
///     表示一个命令的信息。
/// </summary>
/// <remarks>
///     此对象包含命令的信息。这可能包括命令的模块、有关命令的各种描述以及执行模式等。
/// </remarks>
[DebuggerDisplay("{Name,nq}")]
public class CommandInfo
{
    private static readonly MethodInfo _convertParamsMethod =
        typeof(CommandInfo).GetTypeInfo().GetDeclaredMethod(nameof(ConvertParamsList))
        ?? throw new MissingMethodException(nameof(CommandInfo), nameof(ConvertParamsList));

    private static readonly ConcurrentDictionary<Type, Func<IEnumerable<object?>, object?>> _arrayConverters = new();

    private readonly CommandService _commandService;
    private readonly Func<ICommandContext, object?[], IServiceProvider, CommandInfo, Task>? _action;

    /// <summary>
    ///     获取此命令所属的模块。
    /// </summary>
    public ModuleInfo Module { get; }

    /// <summary>
    ///     获取此命令的名称。如果未设置基本名称，则返回首要别名。
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     获取此命令的摘要。
    /// </summary>
    /// <remarks>
    ///     此字段返回命令的摘要。<see cref="Summary"/> 和 <see cref="Remarks"/> 可以用于帮助命令中，为用户提供命令的详细信息。
    /// </remarks>
    public string? Summary { get; }

    /// <summary>
    ///     获取此命令的备注。
    /// </summary>
    /// <remarks>
    ///     此字段返回命令的摘要。<see cref="Summary"/> 和 <see cref="Remarks"/> 可以用于帮助命令中，为用户提供命令的详细信息。
    /// </remarks>
    public string? Remarks { get; }

    /// <summary>
    ///     获取此命令的优先级。当命令匹配多个重载时，此优先级将用于确定要执行的重载。
    /// </summary>
    /// <seealso cref="M:Kook.Commands.PriorityAttribute.#ctor(System.Int32)"/>
    public int Priority { get; }

    /// <summary>
    ///     获取此命令是否可变数量的参数。
    /// </summary>
    public bool HasVarArgs { get; }

    /// <summary>
    ///     获取此命令是否应忽略额外的参数。
    /// </summary>
    public bool IgnoreExtraArgs { get; }

    /// <summary>
    ///     获取此命令的执行模式。
    /// </summary>
    public RunMode RunMode { get; }

    /// <summary>
    ///     获取此命令的所有别名。
    /// </summary>
    public IReadOnlyList<string> Aliases { get; }

    /// <summary>
    ///     获取此命令的所有参数的信息。
    /// </summary>
    public IReadOnlyList<ParameterInfo> Parameters { get; }

    /// <summary>
    ///     获取此命令的所有先决条件。
    /// </summary>
    public IReadOnlyList<PreconditionAttribute> Preconditions { get; }

    /// <summary>
    ///     获取此命令的所有特性。
    /// </summary>
    public IReadOnlyList<Attribute> Attributes { get; }

    internal CommandInfo(CommandBuilder builder, ModuleInfo module, CommandService service)
    {
        Module = module;

        Name = builder.Name ?? string.Empty;
        Summary = builder.Summary;
        Remarks = builder.Remarks;

        RunMode = builder.RunMode == RunMode.Default ? service._defaultRunMode : builder.RunMode;
        Priority = builder.Priority;

        Aliases = module.Aliases
            .Permutate(builder.Aliases, (first, second) =>
            {
                if (first == string.Empty) return second;
                if (second == string.Empty) return first;
                return first + service._separatorChar + second;
            })
            .Select(x => service._caseSensitive ? x : x.ToLowerInvariant())
            .ToImmutableArray();

        Preconditions = builder.Preconditions.ToImmutableArray();
        Attributes = builder.Attributes.ToImmutableArray();

        Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
        HasVarArgs = builder.Parameters.Count > 0 && builder.Parameters[^1].IsMultiple;
        IgnoreExtraArgs = builder.IgnoreExtraArgs;

        _action = builder.Callback;
        _commandService = service;
    }

    /// <summary>
    ///     检查命令在指定的上下文中是否可以执行。
    /// </summary>
    /// <param name="context"> 命令的上下文。 </param>
    /// <param name="services"> 用于检查的服务提供程序。 </param>
    /// <returns> 一个表示异步检查操作的任务。任务的结果包含先决条件的结果。 </returns>
    public async Task<PreconditionResult> CheckPreconditionsAsync(ICommandContext context, IServiceProvider? services = null)
    {
        services ??= EmptyServiceProvider.Instance;

        PreconditionResult moduleResult = await CheckGroups(Module.Preconditions, "Module").ConfigureAwait(false);
        if (!moduleResult.IsSuccess)
            return moduleResult;

        PreconditionResult commandResult = await CheckGroups(Preconditions, "Command").ConfigureAwait(false);
        if (!commandResult.IsSuccess)
            return commandResult;

        return PreconditionResult.FromSuccess();

        async Task<PreconditionResult> CheckGroups(IEnumerable<PreconditionAttribute> preconditions, string type)
        {
            foreach (IGrouping<string?, PreconditionAttribute> preconditionGroup in preconditions.GroupBy(p => p.Group, StringComparer.Ordinal))
            {
                if (preconditionGroup.Key == null)
                {
                    foreach (PreconditionAttribute precondition in preconditionGroup)
                    {
                        PreconditionResult result = await precondition.CheckPermissionsAsync(context, this, services).ConfigureAwait(false);
                        if (!result.IsSuccess) return result;
                    }
                }
                else
                {
                    List<PreconditionResult> results = [];
                    foreach (PreconditionAttribute precondition in preconditionGroup)
                        results.Add(await precondition.CheckPermissionsAsync(context, this, services).ConfigureAwait(false));
                    if (!results.Exists(p => p.IsSuccess))
                        return PreconditionGroupResult.FromError($"{type} precondition group {preconditionGroup.Key} failed.", results);
                }
            }

            return PreconditionGroupResult.FromSuccess();
        }
    }

    /// <summary>
    ///     解析命令的参数。
    /// </summary>
    /// <param name="context"> 命令执行上下文。 </param>
    /// <param name="startIndex"> 解析的起始索引。 </param>
    /// <param name="searchResult"> 命令搜索结果。 </param>
    /// <param name="preconditionResult"> 先决条件的检查结果。 </param>
    /// <param name="services"> 用于解析的服务提供程序。 </param>
    /// <returns> 一个表示异步解析操作的任务。任务的结果包含解析的结果。 </returns>
    public async Task<ParseResult> ParseAsync(ICommandContext context, int startIndex, SearchResult searchResult,
        PreconditionResult? preconditionResult = null, IServiceProvider? services = null)
    {
        services ??= EmptyServiceProvider.Instance;

        if (!searchResult.IsSuccess)
            return ParseResult.FromError(searchResult);
        if (preconditionResult is { IsSuccess: false })
            return ParseResult.FromError(preconditionResult);

        string input = searchResult.Text?[startIndex..] ?? string.Empty;
        return await CommandParser
            .ParseArgsAsync(this, context, _commandService._ignoreExtraArgs, services, input, 0, _commandService._quotationMarkAliasMap)
            .ConfigureAwait(false);
    }

    /// <summary>
    ///     执行命令。
    /// </summary>
    /// <param name="context"> 命令执行上下文。 </param>
    /// <param name="parseResult"> 命令的参数解析结果。 </param>
    /// <param name="services"> 用于执行的服务提供程序。 </param>
    /// <returns> 一个表示异步执行操作的任务。任务的结果包含命令执行的结果。 </returns>
    public Task<IResult> ExecuteAsync(ICommandContext context, ParseResult parseResult, IServiceProvider services)
    {
        if (!parseResult.IsSuccess)
            return Task.FromResult((IResult)ExecuteResult.FromError(parseResult));

        object?[] argList = new object[parseResult.ArgValues.Count];
        for (int i = 0; i < parseResult.ArgValues.Count; i++)
        {
            if (!parseResult.ArgValues[i].IsSuccess)
                return Task.FromResult((IResult)ExecuteResult.FromError(parseResult.ArgValues[i]));
            argList[i] = parseResult.ArgValues[i].Values.First().Value;
        }

        object?[] paramList = new object[parseResult.ParamValues.Count];
        for (int i = 0; i < parseResult.ParamValues.Count; i++)
        {
            if (!parseResult.ParamValues[i].IsSuccess)
                return Task.FromResult((IResult)ExecuteResult.FromError(parseResult.ParamValues[i]));
            paramList[i] = parseResult.ParamValues[i].Values.First().Value;
        }

        return ExecuteAsync(context, argList, paramList, services);
    }

    /// <summary>
    ///     执行命令。
    /// </summary>
    /// <param name="context"> 命令执行上下文。 </param>
    /// <param name="argList"> 命令的实参列表。 </param>
    /// <param name="paramList"> 命令的形参列表。 </param>
    /// <param name="services"> 用于执行的服务提供程序。 </param>
    /// <returns> 一个表示异步执行操作的任务。任务的结果包含命令执行的结果。 </returns>
    public async Task<IResult> ExecuteAsync(ICommandContext context, IEnumerable<object?> argList,
        IEnumerable<object?> paramList, IServiceProvider services)
    {
        services ??= EmptyServiceProvider.Instance;

        try
        {
            object?[] args = GenerateArgs(argList, paramList);

            for (int position = 0; position < Parameters.Count; position++)
            {
                ParameterInfo parameter = Parameters[position];
                object? argument = args[position];
                PreconditionResult result = await parameter.CheckPreconditionsAsync(context, argument, services).ConfigureAwait(false);
                if (!result.IsSuccess)
                {
                    await Module.Service._commandExecutedEvent.InvokeAsync(this, context, result).ConfigureAwait(false);
                    return ExecuteResult.FromError(result);
                }
            }

            switch (RunMode)
            {
                case RunMode.Sync: //Always sync
                    return await ExecuteInternalAsync(context, args, services).ConfigureAwait(false);
                case RunMode.Async: //Always async
                    _ = Task.Run(async () => await ExecuteInternalAsync(context, args, services).ConfigureAwait(false));
                    break;
            }

            return ExecuteResult.FromSuccess();
        }
        catch (Exception ex)
        {
            return ExecuteResult.FromError(ex);
        }
    }

    private async Task<IResult> ExecuteInternalAsync(ICommandContext context, object?[] args, IServiceProvider services)
    {
        await Module.Service._cmdLogger.DebugAsync($"Executing {GetLogText(context)}").ConfigureAwait(false);
        try
        {
            Task? task =  _action?.Invoke(context, args, services, this);
            if (task is Task<IResult> resultTask)
            {
                IResult result = await resultTask.ConfigureAwait(false);
                await Module.Service._commandExecutedEvent.InvokeAsync(this, context, result).ConfigureAwait(false);
                if (result is RuntimeResult execResult)
                    return execResult;
            }
            else if (task is Task<ExecuteResult> execTask)
            {
                ExecuteResult result = await execTask.ConfigureAwait(false);
                await Module.Service._commandExecutedEvent.InvokeAsync(this, context, result).ConfigureAwait(false);
                return result;
            }
            else
            {
                if (task != null)
                    await task.ConfigureAwait(false);
                ExecuteResult result = ExecuteResult.FromSuccess();
                await Module.Service._commandExecutedEvent.InvokeAsync(this, context, result).ConfigureAwait(false);
            }

            ExecuteResult executeResult = ExecuteResult.FromSuccess();
            return executeResult;
        }
        catch (Exception ex)
        {
            Exception? internalEx = ex;
            while (internalEx is TargetInvocationException) //Happens with void-returning commands
                internalEx = internalEx.InnerException;

            CommandException wrappedEx = new(this, context, internalEx);
            await Module.Service._cmdLogger.ErrorAsync(wrappedEx).ConfigureAwait(false);

            ExecuteResult result = ExecuteResult.FromError(internalEx);
            await Module.Service._commandExecutedEvent.InvokeAsync(this, context, result).ConfigureAwait(false);

            if (Module.Service._throwOnError)
            {
                if (internalEx == ex)
                    throw;
                else
                    ExceptionDispatchInfo.Capture(internalEx ?? ex).Throw();
            }

            return result;
        }
        finally
        {
            await Module.Service._cmdLogger.VerboseAsync($"Executed {GetLogText(context)}").ConfigureAwait(false);
        }
    }

    private object?[] GenerateArgs(IEnumerable<object?> argList, IEnumerable<object?> paramsList)
    {
        int argCount = Parameters.Count;
        object?[] array = new object?[Parameters.Count];
        if (HasVarArgs)
            argCount--;

        int i = 0;
        foreach (object? arg in argList)
        {
            if (i == argCount)
                throw new InvalidOperationException("Command was invoked with too many parameters.");
            array[i++] = arg;
        }

        if (i < argCount)
            throw new InvalidOperationException("Command was invoked with too few parameters.");

        if (HasVarArgs && Parameters[^1].Type is { } argType)
        {
            Func<IEnumerable<object?>, object?> func = _arrayConverters.GetOrAdd(argType, t =>
            {
                MethodInfo method = _convertParamsMethod.MakeGenericMethod(t);
                return (Func<IEnumerable<object?>, object?>)method.CreateDelegate(typeof(Func<IEnumerable<object?>, object?>));
            });
            array[i] = func(paramsList);
        }

        return array;
    }

    private static T[] ConvertParamsList<T>(IEnumerable<object> paramsList) =>
        paramsList.Cast<T>().ToArray();

    internal string GetLogText(ICommandContext context) =>
        context.Guild != null
            ? $"\"{Name}\" for {context.User} in {context.Guild}/{context.Channel}"
            : $"\"{Name}\" for {context.User} in {context.Channel}";
}
