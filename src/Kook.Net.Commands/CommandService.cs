using Kook.Commands.Builders;
using Kook.Logging;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;

namespace Kook.Commands;

/// <summary>
///     表示一个基于文本的命令服务。
/// </summary>
/// <remarks>
///     此类用于支持在运行时动态创建命令，或者在编译时静态创建命令。要在编译时创建命令模块，参见
///     <see cref="T:Kook.Commands.ModuleBase"/> 或 <see cref="T:Kook.Commands.Builders.ModuleBuilder"/>。 <br />
///     此服务还提供了几个事件，用于监视命令的使用情况；例如 <see cref="M:Kook.Commands.CommandService.Log" /> 用于任何与命令相关的日志事件，
/// </remarks>
public class CommandService : IDisposable
{
    #region CommandService

    /// <summary>
    ///     当产生与命令相关的日志信息时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.LogMessage"/> 参数是日志消息。 </item>
    ///     </list>
    /// </remarks>
    public event Func<LogMessage, Task> Log
    {
        add => _logEvent.Add(value);
        remove => _logEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new();

    /// <summary>
    ///     当命令执行时引发。
    /// </summary>
    /// <remarks>
    ///     此事件在命令执行后引发，既包含了执行成功的情况，也包含了执行失败的情况。
    ///     <br />
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:Kook.Commands.CommandInfo"/>? 参数是执行的命令，如果命令在解析或先决条件阶段失败，则可能为 <c>null</c>。 </item>
    ///     <item> <see cref="T:Kook.Commands.ICommandContext"/> 参数是命令的上下文。 </item>
    ///     <item> <see cref="T:Kook.Commands.IResult"/> 参数是命令的结果。 </item>
    ///     </list>
    /// </remarks>
    public event Func<CommandInfo?, ICommandContext, IResult, Task> CommandExecuted
    {
        add => _commandExecutedEvent.Add(value);
        remove => _commandExecutedEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<CommandInfo?, ICommandContext, IResult, Task>> _commandExecutedEvent = new();

    private readonly SemaphoreSlim _moduleLock;
    private readonly ConcurrentDictionary<Type, ModuleInfo> _typedModuleDefs;
    private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, TypeReader>> _typeReaders;
    private readonly ConcurrentDictionary<Type, TypeReader> _defaultTypeReaders;
    private readonly ImmutableList<(Type EntityType, Type TypeReaderType)> _entityTypeReaders;
    private readonly HashSet<ModuleInfo> _moduleDefs;
    private readonly CommandMap _map;

    internal readonly bool _caseSensitive, _throwOnError, _ignoreExtraArgs;
    internal readonly char _separatorChar;
    internal readonly RunMode _defaultRunMode;
    internal readonly Logger _cmdLogger;
    internal readonly LogManager _logManager;
    internal readonly IReadOnlyDictionary<char, char> _quotationMarkAliasMap;

    internal bool _isDisposed;

    /// <summary>
    ///     获取所有加载的模块。
    /// </summary>
    public IEnumerable<ModuleInfo> Modules => _moduleDefs.Select(x => x);

    /// <summary>
    ///     获取所有加载的命令。
    /// </summary>
    public IEnumerable<CommandInfo> Commands => _moduleDefs.SelectMany(x => x.Commands);

    /// <summary>
    ///     获取所有加载的类型读取器。
    /// </summary>
    public ILookup<Type, TypeReader> TypeReaders => _typeReaders
        .SelectMany(x => x.Value.Select(y => new { y.Key, y.Value }))
        .ToLookup(x => x.Key, x => x.Value);

    /// <summary>
    ///     初始化一个 <see cref="CommandService"/> 类的新实例。
    /// </summary>
    public CommandService() : this(new CommandServiceConfig())
    {
    }

    /// <summary>
    ///     初始化一个 <see cref="CommandService"/> 类的新实例。
    /// </summary>
    /// <param name="config"> 命令服务的配置。 </param>
    /// <exception cref="InvalidOperationException"> 默认运行模式不能设置为 <see cref="F:Kook.Commands.RunMode.Default"/>。 </exception>
    public CommandService(CommandServiceConfig config)
    {
        _caseSensitive = config.CaseSensitiveCommands;
        _throwOnError = config.ThrowOnError;
        _ignoreExtraArgs = config.IgnoreExtraArgs;
        _separatorChar = config.SeparatorChar;
        _defaultRunMode = config.DefaultRunMode;
        _quotationMarkAliasMap = config.QuotationMarkAliasMap.ToImmutableDictionary();
        if (_defaultRunMode == RunMode.Default)
            throw new InvalidOperationException("The default run mode cannot be set to Default.");

        _logManager = new LogManager(config.LogLevel);
        _logManager.Message += async msg => await _logEvent.InvokeAsync(msg).ConfigureAwait(false);
        _cmdLogger = _logManager.CreateLogger("Command");

        _moduleLock = new SemaphoreSlim(1, 1);
        _typedModuleDefs = new ConcurrentDictionary<Type, ModuleInfo>();
        _moduleDefs = new HashSet<ModuleInfo>();
        _map = new CommandMap(this);
        _typeReaders = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, TypeReader>>();

        _defaultTypeReaders = new ConcurrentDictionary<Type, TypeReader>();
        foreach (Type type in PrimitiveParsers.SupportedTypes)
        {
            if (PrimitiveTypeReader.Create(type) is { } typeReader)
                _defaultTypeReaders[type] = typeReader;
            _defaultTypeReaders[typeof(Nullable<>).MakeGenericType(type)] = NullableTypeReader.Create(type, _defaultTypeReaders[type]);
        }

        TimeSpanTypeReader tsreader = new();
        _defaultTypeReaders[typeof(TimeSpan)] = tsreader;
        _defaultTypeReaders[typeof(TimeSpan?)] = NullableTypeReader.Create(typeof(TimeSpan), tsreader);
        UriTypeReader uriReader = new();
        _defaultTypeReaders[typeof(Uri)] = uriReader;

        _defaultTypeReaders[typeof(string)] =
            new PrimitiveTypeReader<string>((string x, out string y) =>
            {
                y = x;
                return true;
            }, 0);

        ImmutableList<(Type, Type)>.Builder entityTypeReaders = ImmutableList.CreateBuilder<(Type, Type)>();
        entityTypeReaders.Add((typeof(IMessage), typeof(MessageTypeReader<>)));
        entityTypeReaders.Add((typeof(IChannel), typeof(ChannelTypeReader<>)));
        entityTypeReaders.Add((typeof(IRole), typeof(RoleTypeReader<>)));
        entityTypeReaders.Add((typeof(IUser), typeof(UserTypeReader<>)));
        _entityTypeReaders = entityTypeReaders.ToImmutable();
    }

    #endregion

    #region Modules

    /// <summary>
    ///     创建一个命令模块。
    /// </summary>
    /// <param name="primaryAlias"> 模块的首要别名。 </param>
    /// <param name="buildFunc"> 一个构建模块的委托。 </param>
    /// <returns> 一个表示异步操作的任务。任务结果包含构建的模块。 </returns>
    public async Task<ModuleInfo> CreateModuleAsync(string primaryAlias, Action<ModuleBuilder> buildFunc)
    {
        await _moduleLock.WaitAsync().ConfigureAwait(false);
        try
        {
            ModuleBuilder builder = new(this, null, primaryAlias);
            buildFunc(builder);
            ModuleInfo module = builder.Build(this, EmptyServiceProvider.Instance);
            return LoadModuleInternal(module);
        }
        finally
        {
            _moduleLock.Release();
        }
    }

    /// <summary>
    ///     添加一个命令模块。
    /// </summary>
    /// <typeparam name="T"> 要添加的模块的类型。 </typeparam>
    /// <param name="services"> 用于依赖注入的服务提供程序；如果不使用依赖注入，则传递 <c>null</c>。 </param>
    /// <returns> 一个表示异步添加操作的任务。任务结果包含添加的模块。 </returns>
    /// <exception cref="ArgumentException"> 此模块已经添加。 </exception>
    /// <exception cref="InvalidOperationException"> 无法构建 <see cref="T:Kook.Commands.ModuleInfo"/>；可能提供了无效的类型。 </exception>
    /// <example>
    ///     以下示例代码将模块 <c>MyModule</c> 注册到 <c>commandService</c> 中。
    ///     <code language="cs">
    ///         await commandService.AddModuleAsync&lt;MyModule&gt;(serviceProvider);
    ///     </code>
    /// </example>
    public Task<ModuleInfo> AddModuleAsync<T>(IServiceProvider? services) => AddModuleAsync(typeof(T), services);

    /// <summary>
    ///     添加一个命令模块。
    /// </summary>
    /// <param name="type"> 要添加的模块的类型。 </param>
    /// <param name="services"> 用于依赖注入的服务提供程序；如果不使用依赖注入，则传递 <c>null</c>。 </param>
    /// <returns> 一个表示异步添加操作的任务。任务结果包含添加的模块。 </returns>
    /// <exception cref="ArgumentException"> 此模块已经添加。 </exception>
    /// <exception cref="InvalidOperationException"> 无法构建 <see cref="T:Kook.Commands.ModuleInfo"/>；可能提供了无效的类型。 </exception>
    public async Task<ModuleInfo> AddModuleAsync(Type type, IServiceProvider? services)
    {
        services ??= EmptyServiceProvider.Instance;
        await _moduleLock.WaitAsync().ConfigureAwait(false);
        try
        {
            TypeInfo typeInfo = type.GetTypeInfo();

            if (_typedModuleDefs.ContainsKey(type)) throw new ArgumentException("This module has already been added.");

            Dictionary<Type, ModuleInfo> moduleInfos = await ModuleClassBuilder
                .BuildAsync(this, services, typeInfo)
                .ConfigureAwait(false);
            KeyValuePair<Type, ModuleInfo> module = moduleInfos.FirstOrDefault();

            _typedModuleDefs[module.Key] = module.Value
                ?? throw new InvalidOperationException($"Could not build the module {type.FullName}, did you pass an invalid type?");

            return LoadModuleInternal(module.Value);
        }
        finally
        {
            _moduleLock.Release();
        }
    }

    /// <summary>
    ///     添加程序集内的所有命令模块。
    /// </summary>
    /// <param name="assembly"> 要添加其所有模块的程序集。 </param>
    /// <param name="services"> 用于依赖注入的服务提供程序；如果不使用依赖注入，则传递 <c>null</c>。 </param>
    /// <returns> 一个表示异步添加操作的任务。任务结果包含所有添加的模块。 </returns>
    public async Task<IEnumerable<ModuleInfo>> AddModulesAsync(Assembly assembly, IServiceProvider? services)
    {
        services ??= EmptyServiceProvider.Instance;

        await _moduleLock.WaitAsync().ConfigureAwait(false);
        try
        {
            IReadOnlyList<TypeInfo> types = await ModuleClassBuilder.SearchAsync(assembly, this).ConfigureAwait(false);
            Dictionary<Type, ModuleInfo> moduleDefs = await ModuleClassBuilder.BuildAsync(types, this, services).ConfigureAwait(false);

            foreach (KeyValuePair<Type, ModuleInfo> info in moduleDefs)
            {
                _typedModuleDefs[info.Key] = info.Value;
                LoadModuleInternal(info.Value);
            }

            return moduleDefs.Select(x => x.Value).ToImmutableArray();
        }
        finally
        {
            _moduleLock.Release();
        }
    }

    private ModuleInfo LoadModuleInternal(ModuleInfo module)
    {
        _moduleDefs.Add(module);
        foreach (CommandInfo command in module.Commands)
            _map.AddCommand(command);
        foreach (ModuleInfo submodule in module.Submodules)
            LoadModuleInternal(submodule);
        return module;
    }

    /// <summary>
    ///     移除命令模块。
    /// </summary>
    /// <param name="module"> 要移除的模块。 </param>
    /// <returns> 一个表示异步删除操作的任务。如果任务结果为 <c>true</c>，则表示模块已成功删除，否则表示模块不存在。 </returns>
    public async Task<bool> RemoveModuleAsync(ModuleInfo module)
    {
        await _moduleLock.WaitAsync().ConfigureAwait(false);
        try
        {
            return RemoveModuleInternal(module);
        }
        finally
        {
            _moduleLock.Release();
        }
    }

    /// <summary>
    ///     移除命令模块。
    /// </summary>
    /// <typeparam name="T"> 要移除的模块的类型。 </typeparam>
    /// <returns> 一个表示异步删除操作的任务。如果任务结果为 <c>true</c>，则表示模块已成功删除，否则表示模块不存在。 </returns>
    public Task<bool> RemoveModuleAsync<T>() => RemoveModuleAsync(typeof(T));

    /// <summary>
    ///     移除命令模块。
    /// </summary>
    /// <param name="type"> 要移除的模块的类型。 </param>
    /// <returns> 一个表示异步删除操作的任务。如果任务结果为 <c>true</c>，则表示模块已成功删除，否则表示模块不存在。 </returns>
    public async Task<bool> RemoveModuleAsync(Type type)
    {
        await _moduleLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!_typedModuleDefs.TryRemove(type, out ModuleInfo? module))
                return false;
            return RemoveModuleInternal(module);
        }
        finally
        {
            _moduleLock.Release();
        }
    }

    private bool RemoveModuleInternal(ModuleInfo module)
    {
        if (!_moduleDefs.Remove(module)) return false;
        foreach (CommandInfo cmd in module.Commands)
            _map.RemoveCommand(cmd);
        foreach (ModuleInfo submodule in module.Submodules)
            RemoveModuleInternal(submodule);
        return true;
    }

    #endregion

    #region Type Readers

    /// <summary>
    ///     添加一个自定义的类型读取器。
    /// </summary>
    /// <remarks>
    ///     如果 <typeparamref name="T" /> 是一个值类型，那么一个读取对应可空值类型的 <see cref="TypeReader" /> 也会被添加。 <br />
    ///     如果 <typeparamref name="T" /> 的默认 <see cref="TypeReader" /> 已经存在，那么会记录一个警告，默认的 <see cref="TypeReader" /> 将会被替换。
    /// </remarks>
    /// <typeparam name="T"> 要读取的对象类型。 </typeparam>
    /// <param name="reader"> 要添加的类型读取器的实例。 </param>
    public void AddTypeReader<T>(TypeReader reader) => AddTypeReader(typeof(T), reader);

    /// <summary>
    ///     添加一个自定义的类型读取器。
    /// </summary>
    /// <remarks>
    ///     如果 <paramref name="type" /> 是一个值类型，那么一个读取对应可空值类型的 <see cref="TypeReader" /> 也会被添加。 <br />
    ///     如果 <paramref name="type" /> 的默认 <see cref="TypeReader" /> 已经存在，那么会记录一个警告，默认的 <see cref="TypeReader" /> 将会被替换。
    /// </remarks>
    /// <param name="type"> 要读取的对象类型。 </param>
    /// <param name="reader"> 要添加的类型读取器的实例。 </param>
    public void AddTypeReader(Type type, TypeReader reader)
    {
        if (_defaultTypeReaders.ContainsKey(type))
        {
            _ = _cmdLogger.WarningAsync(
                $"The default TypeReader for {type.FullName} was replaced by {reader.GetType().FullName}. "
                + "To suppress this message, use AddTypeReader<T>(reader, true).");
        }

        AddTypeReader(type, reader, true);
    }

    /// <summary>
    ///     添加一个自定义的类型读取器。
    /// </summary>
    /// <remarks>
    ///     如果 <typeparamref name="T" /> 是一个值类型，那么一个读取对应可空值类型的 <see cref="TypeReader" /> 也会被添加。
    /// </remarks>
    /// <typeparam name="T"> 要读取的对象类型。 </typeparam>
    /// <param name="reader"> 要添加的类型读取器的实例。 </param>
    /// <param name="replaceDefault"> 是否替换默认的 <see cref="TypeReader"/>。 </param>
    public void AddTypeReader<T>(TypeReader reader, bool replaceDefault) =>
        AddTypeReader(typeof(T), reader, replaceDefault);

    /// <summary>
    ///     添加一个自定义的类型读取器。
    /// </summary>
    /// <remarks>
    ///     如果 <paramref name="type" /> 是一个值类型，那么一个读取对应可空值类型的 <see cref="TypeReader" /> 也会被添加。
    /// </remarks>
    /// <param name="type"> 要读取的对象类型。 </param>
    /// <param name="reader"> 要添加的类型读取器的实例。 </param>
    /// <param name="replaceDefault"> 是否替换默认的 <see cref="TypeReader"/>。 </param>
    public void AddTypeReader(Type type, TypeReader reader, bool replaceDefault)
    {
        if (replaceDefault && HasDefaultTypeReader(type))
        {
            _defaultTypeReaders.AddOrUpdate(type, reader, (_, _) => reader);
            if (type.GetTypeInfo().IsValueType)
            {
                Type nullableType = typeof(Nullable<>).MakeGenericType(type);
                TypeReader nullableReader = NullableTypeReader.Create(type, reader);
                _defaultTypeReaders.AddOrUpdate(nullableType, nullableReader, (_, _) => nullableReader);
            }
        }
        else
        {
            ConcurrentDictionary<Type, TypeReader> readers = _typeReaders.GetOrAdd(type, _ => new ConcurrentDictionary<Type, TypeReader>());
            readers[reader.GetType()] = reader;
            if (type.GetTypeInfo().IsValueType) AddNullableTypeReader(type, reader);
        }
    }

    /// <summary>
    ///     移除一个类型读取器。
    /// </summary>
    /// <remarks>
    ///     从命令服务中移除一个 <see cref="TypeReader"/> 不会从已加载的模块与命令的实例中取消引用
    ///     <see cref="TypeReader"/>。要使更改生效，您需要重新加载模块。
    /// </remarks>
    /// <param name="type"> 要移除的类型读取器所读取的对象类型。 </param>
    /// <param name="isDefaultTypeReader"> 是否要移除默认的 <see cref="TypeReader"/>。 </param>
    /// <param name="readers"> 移除的类型读取器。 </param>
    /// <returns> 如果成功移除，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public bool TryRemoveTypeReader(Type type, bool isDefaultTypeReader, out IDictionary<Type, TypeReader> readers)
    {
        readers = new Dictionary<Type, TypeReader>();

        if (isDefaultTypeReader)
        {
            if (!_defaultTypeReaders.TryRemove(type, out TypeReader? result))
                return false;
            readers.Add(result.GetType(), result);
            return true;
        }
        else
        {
            if (!_typeReaders.TryRemove(type, out ConcurrentDictionary<Type, TypeReader>? result))
                return false;
            readers = result;
            return true;
        }
    }

    internal bool HasDefaultTypeReader(Type type)
    {
        if (_defaultTypeReaders.ContainsKey(type))
            return true;
        TypeInfo typeInfo = type.GetTypeInfo();
        if (typeInfo.IsEnum)
            return true;
        return _entityTypeReaders.Exists(x => type == x.EntityType || typeInfo.ImplementedInterfaces.Contains(x.EntityType));
    }

    internal void AddNullableTypeReader(Type valueType, TypeReader valueTypeReader)
    {
        ConcurrentDictionary<Type, TypeReader> readers = _typeReaders
            .GetOrAdd(typeof(Nullable<>).MakeGenericType(valueType), _ => new ConcurrentDictionary<Type, TypeReader>());
        TypeReader nullableReader = NullableTypeReader.Create(valueType, valueTypeReader);
        readers[nullableReader.GetType()] = nullableReader;
    }

    internal IDictionary<Type, TypeReader>? GetTypeReaders(Type? type)
    {
        if (type == null) return null;
        if (!_typeReaders.TryGetValue(type, out ConcurrentDictionary<Type, TypeReader>? definedTypeReaders)) return null;
        return definedTypeReaders;
    }

    internal TypeReader? GetDefaultTypeReader(Type? type)
    {
        if (type == null) return null;
        if (_defaultTypeReaders.TryGetValue(type, out TypeReader? reader))
            return reader;

        TypeInfo typeInfo = type.GetTypeInfo();

        //Is this an enum?
        if (typeInfo.IsEnum)
        {
            reader = EnumTypeReader.GetReader(type);
            _defaultTypeReaders[type] = reader;
            return reader;
        }

        Type? underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType is { IsEnum: true })
        {
            reader = NullableTypeReader.Create(underlyingType, EnumTypeReader.GetReader(underlyingType));
            _defaultTypeReaders[type] = reader;
            return reader;
        }

        //Is this an entity?
        for (int i = 0; i < _entityTypeReaders.Count; i++)
        {
            if (type == _entityTypeReaders[i].EntityType
                || typeInfo.ImplementedInterfaces.Contains(_entityTypeReaders[i].EntityType))
            {
                reader = Activator.CreateInstance(_entityTypeReaders[i].TypeReaderType.MakeGenericType(type)) as TypeReader;
                if (reader is not null)
                    _defaultTypeReaders[type] = reader;
                return reader;
            }
        }

        return null;
    }

    #endregion

    #region Execution

    /// <summary>
    ///     搜索命令。
    /// </summary>
    /// <param name="context"> 命令的上下文。 </param>
    /// <param name="argPos"> 命令的位置。 </param>
    /// <returns> 命令搜索的结果。 </returns>
    public SearchResult Search(ICommandContext context, int argPos) =>
        Search(context.Message.Content.Substring(argPos));

    /// <summary>
    ///     搜索命令。
    /// </summary>
    /// <param name="context"> 命令的上下文。 </param>
    /// <param name="input"> 命令字符串。 </param>
    /// <returns> 命令搜索的结果。 </returns>
    public SearchResult Search(ICommandContext context, string input) => Search(input);

    /// <summary>
    ///     搜索命令。
    /// </summary>
    /// <param name="input"> 命令字符串。 </param>
    /// <returns> 命令搜索的结果。 </returns>
    public SearchResult Search(string input)
    {
        string searchInput = _caseSensitive ? input : input.ToLowerInvariant();
        ImmutableArray<CommandMatch> matches = [.._map.GetCommands(searchInput).OrderByDescending(x => x.Command.Priority)];

        return matches.Length > 0
            ? SearchResult.FromSuccess(input, matches)
            : SearchResult.FromError(CommandError.UnknownCommand, "Unknown command.");
    }

    /// <summary>
    ///     执行命令。
    /// </summary>
    /// <param name="context"> 命令的上下文。 </param>
    /// <param name="argPos"> 命令的位置。 </param>
    /// <param name="services"> 要用于命令执行的依赖注入服务。 </param>
    /// <param name="multiMatchHandling"> 当匹配到多个命令时的处理模式。 </param>
    /// <returns>
    ///     一个表示异步执行操作的任务。任务的结果包含执行的结果。
    /// </returns>
    public Task<IResult> ExecuteAsync(ICommandContext context, int argPos, IServiceProvider services,
        MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception)
    {
        string input = context.Message.TryExpandCardContent(out string? expandedContent)
            ? expandedContent[argPos..]
            : context.Message.Content[argPos..];
        return ExecuteAsync(context, input, services, multiMatchHandling);
    }

    /// <summary>
    ///     执行命令。
    /// </summary>
    /// <param name="context"> 命令的上下文。 </param>
    /// <param name="input"> 命令字符串。 </param>
    /// <param name="services"> 要用于命令执行的依赖注入服务。 </param>
    /// <param name="multiMatchHandling"> 当匹配到多个命令时的处理模式。 </param>
    /// <returns>
    ///     一个表示异步执行操作的任务。任务的结果包含执行的结果。
    /// </returns>
    public async Task<IResult> ExecuteAsync(ICommandContext context, string input, IServiceProvider services,
        MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception)
    {
        services ??= EmptyServiceProvider.Instance;
        SearchResult searchResult = Search(input);
        IResult validationResult = await ValidateAndGetBestMatch(searchResult, context, services, multiMatchHandling);
        if (validationResult is SearchResult result)
        {
            await _commandExecutedEvent.InvokeAsync(null, context, result).ConfigureAwait(false);
            return result;
        }
        if (validationResult is MatchResult matchResult)
            return await HandleCommandPipeline(matchResult, context, services);
        return validationResult;
    }

    private async Task<IResult> HandleCommandPipeline(MatchResult matchResult, ICommandContext context, IServiceProvider services)
    {
        if (!matchResult.IsSuccess)
            return matchResult;

        if (matchResult.Pipeline is ParseResult parseResult)
        {
            if (!parseResult.IsSuccess || !matchResult.Match.HasValue)
            {
                await _commandExecutedEvent.InvokeAsync(matchResult.Match?.Command, context, parseResult);
                return parseResult;
            }

            IResult executeResult = await matchResult.Match.Value.ExecuteAsync(context, parseResult, services);

            if (!executeResult.IsSuccess
                && executeResult is not (RuntimeResult or ExecuteResult))
                // succesful results raise the event in CommandInfo#ExecuteInternalAsync (have to raise it there b/c deffered execution)
                await _commandExecutedEvent.InvokeAsync(matchResult.Match.Value.Command, context, executeResult);

            return executeResult;
        }

        if (matchResult is { Pipeline: PreconditionResult preconditionResult, Match: not null })
        {
            await _commandExecutedEvent.InvokeAsync(matchResult.Match.Value.Command, context, preconditionResult).ConfigureAwait(false);
            return preconditionResult;
        }

        return matchResult;
    }

    // Calculates the 'score' of a command given a parse result
    private float CalculateScore(CommandMatch match, ParseResult parseResult)
    {
        float argValuesScore = 0;
        float paramValuesScore = 0;

        if (match.Command.Parameters.Count > 0)
        {
            float argValuesSum = parseResult.ArgValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;
            float paramValuesSum = parseResult.ParamValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;

            argValuesScore = argValuesSum / match.Command.Parameters.Count;
            paramValuesScore = paramValuesSum / match.Command.Parameters.Count;
        }

        float totalArgsScore = (argValuesScore + paramValuesScore) / 2;
        return match.Command.Priority + totalArgsScore * 0.99f;
    }

    /// <summary>
    ///     从指定的搜索结果中验证先决条件并获取最佳匹配。
    /// </summary>
    /// <param name="matches"> 要验证的搜索结果。 </param>
    /// <param name="context"> 命令的上下文。 </param>
    /// <param name="provider"> 要用于命令验证与解析的依赖注入服务。 </param>
    /// <param name="multiMatchHandling"> 当匹配到多个命令时的处理模式。 </param>
    /// <returns> 一个表示异步操作的任务。任务的结果包含验证与最佳匹配的结果。 </returns>
    public async Task<IResult> ValidateAndGetBestMatch(SearchResult matches, ICommandContext context, IServiceProvider provider,
        MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception)
    {
        if (!matches.IsSuccess) return matches;

        IReadOnlyList<CommandMatch> commands = matches.Commands;
        Dictionary<CommandMatch, PreconditionResult> preconditionResults = new();

        foreach (CommandMatch command in commands)
            preconditionResults[command] = await command.CheckPreconditionsAsync(context, provider);

        KeyValuePair<CommandMatch, PreconditionResult>[] successfulPreconditions = preconditionResults
            .Where(x => x.Value.IsSuccess)
            .ToArray();

        if (successfulPreconditions.Length == 0)
        {
            //All preconditions failed, return the one from the highest priority command
            KeyValuePair<CommandMatch, PreconditionResult> bestCandidate = preconditionResults
                .OrderByDescending(x => x.Key.Command.Priority)
                .FirstOrDefault(x => !x.Value.IsSuccess);
            return MatchResult.FromSuccess(bestCandidate.Key, bestCandidate.Value);
        }

        Dictionary<CommandMatch, ParseResult> parseResults = new();

        foreach (KeyValuePair<CommandMatch, PreconditionResult> pair in successfulPreconditions)
        {
            ParseResult parseResult = await pair.Key.ParseAsync(context, matches, pair.Value, provider).ConfigureAwait(false);

            if (parseResult.Error == CommandError.MultipleMatches)
            {
                IReadOnlyList<TypeReaderValue> argList, paramList;
                if (multiMatchHandling == MultiMatchHandling.Best)
                {
                    argList = parseResult.ArgValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
                    paramList = parseResult.ParamValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
                    parseResult = ParseResult.FromSuccess(argList, paramList);
                }
            }

            parseResults[pair.Key] = parseResult;
        }

        IOrderedEnumerable<KeyValuePair<CommandMatch, ParseResult>> weightedParseResults = parseResults
            .OrderByDescending(x => CalculateScore(x.Key, x.Value));

        KeyValuePair<CommandMatch, ParseResult>[] successfulParses = weightedParseResults
            .Where(x => x.Value.IsSuccess)
            .ToArray();

        if (successfulParses.Length == 0)
        {
            KeyValuePair<CommandMatch, ParseResult> bestMatch = parseResults
                .FirstOrDefault(x => !x.Value.IsSuccess);

            return MatchResult.FromSuccess(bestMatch.Key, bestMatch.Value);
        }

        KeyValuePair<CommandMatch, ParseResult> chosenOverload = successfulParses[0];

        return MatchResult.FromSuccess(chosenOverload.Key, chosenOverload.Value);
    }

    #endregion

    #region Dispose

    /// <inheritdoc cref="IDisposable.Dispose" />
    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing) _moduleLock?.Dispose();

            _isDisposed = true;
        }
    }

    /// <inheritdoc />
    void IDisposable.Dispose() => Dispose(true);

    #endregion
}
