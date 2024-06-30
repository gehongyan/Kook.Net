using Kook.Commands.Builders;
using Kook.Logging;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;

namespace Kook.Commands;

/// <summary>
///     Provides a framework for building Kook commands.
/// </summary>
/// <remarks>
///     <para>
///         The service provides a framework for building Kook commands both dynamically via runtime builders or
///         statically via compile-time modules. To create a command module at compile-time, see
///         <see cref="ModuleBase" /> (most common); otherwise, see <see cref="ModuleBuilder" />.
///     </para>
///     <para>
///         This service also provides several events for monitoring command usages; such as
///         <see cref="Kook.Commands.CommandService.Log" /> for any command-related log events, and
///         <see cref="Kook.Commands.CommandService.CommandExecuted" /> for information about commands that have
///         been successfully executed.
///     </para>
/// </remarks>
public class CommandService : IDisposable
{
    #region CommandService

    /// <summary>
    ///     Occurs when a command-related information is received.
    /// </summary>
    public event Func<LogMessage, Task> Log
    {
        add => _logEvent.Add(value);
        remove => _logEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new();

    /// <summary>
    ///     Occurs when a command is executed.
    /// </summary>
    /// <remarks>
    ///     This event is fired when a command has been executed, successfully or not. When a command fails to
    ///     execute during parsing or precondition stage, the CommandInfo may not be returned.
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
    ///     Represents all modules loaded within <see cref="CommandService"/>.
    /// </summary>
    public IEnumerable<ModuleInfo> Modules => _moduleDefs.Select(x => x);

    /// <summary>
    ///     Represents all commands loaded within <see cref="CommandService"/>.
    /// </summary>
    public IEnumerable<CommandInfo> Commands => _moduleDefs.SelectMany(x => x.Commands);

    /// <summary>
    ///     Represents all <see cref="TypeReader" /> loaded within <see cref="CommandService"/>.
    /// </summary>
    public ILookup<Type, TypeReader> TypeReaders => _typeReaders
        .SelectMany(x => x.Value.Select(y => new { y.Key, y.Value }))
        .ToLookup(x => x.Key, x => x.Value);

    /// <summary>
    ///     Initializes a new <see cref="CommandService"/> class.
    /// </summary>
    public CommandService() : this(new CommandServiceConfig())
    {
    }

    /// <summary>
    ///     Initializes a new <see cref="CommandService"/> class with the provided configuration.
    /// </summary>
    /// <param name="config">The configuration class.</param>
    /// <exception cref="InvalidOperationException">
    /// The <see cref="RunMode"/> cannot be set to <see cref="RunMode.Default"/>.
    /// </exception>
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
    ///     Creates a new module builder.
    /// </summary>
    /// <param name="primaryAlias"> The primary alias for the module. </param>
    /// <param name="buildFunc"> The action delegate to build the module. </param>
    /// <returns> A task that represents the asynchronous operation for creating the module. </returns>
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
    ///     Add a command module from a <see cref="Type" />.
    /// </summary>
    /// <example>
    ///     <para>The following example registers the module <c>MyModule</c> to <c>commandService</c>.</para>
    ///     <code language="cs">
    ///     await commandService.AddModuleAsync&lt;MyModule&gt;(serviceProvider);
    ///     </code>
    /// </example>
    /// <typeparam name="T">The type of module.</typeparam>
    /// <param name="services">The <see cref="IServiceProvider"/> for your dependency injection solution if using one; otherwise, pass <c>null</c>.</param>
    /// <exception cref="ArgumentException">This module has already been added.</exception>
    /// <exception cref="InvalidOperationException">
    /// The <see cref="ModuleInfo"/> fails to be built; an invalid type may have been provided.
    /// </exception>
    /// <returns>
    ///     A task that represents the asynchronous operation for adding the module. The task result contains the
    ///     built module.
    /// </returns>
    public Task<ModuleInfo> AddModuleAsync<T>(IServiceProvider services) => AddModuleAsync(typeof(T), services);

    /// <summary>
    ///     Adds a command module from a <see cref="Type" />.
    /// </summary>
    /// <param name="type">The type of module.</param>
    /// <param name="services">The <see cref="IServiceProvider" /> for your dependency injection solution if using one; otherwise, pass <c>null</c> .</param>
    /// <exception cref="ArgumentException">This module has already been added.</exception>
    /// <exception cref="InvalidOperationException">
    /// The <see cref="ModuleInfo"/> fails to be built; an invalid type may have been provided.
    /// </exception>
    /// <returns>
    ///     A task that represents the asynchronous operation for adding the module. The task result contains the
    ///     built module.
    /// </returns>
    public async Task<ModuleInfo> AddModuleAsync(Type type, IServiceProvider services)
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
    ///     Add command modules from an <see cref="Assembly"/>.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly"/> containing command modules.</param>
    /// <param name="services">The <see cref="IServiceProvider"/> for your dependency injection solution if using one; otherwise, pass <c>null</c>.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation for adding the command modules. The task result
    ///     contains an enumerable collection of modules added.
    /// </returns>
    public async Task<IEnumerable<ModuleInfo>> AddModulesAsync(Assembly assembly, IServiceProvider services)
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
    ///     Removes the command module.
    /// </summary>
    /// <param name="module">The <see cref="ModuleInfo" /> to be removed from the service.</param>
    /// <returns>
    ///     A task that represents the asynchronous removal operation. The task result contains a value that
    ///     indicates whether the <paramref name="module"/> is successfully removed.
    /// </returns>
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
    ///     Removes the command module.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the module.</typeparam>
    /// <returns>
    ///     A task that represents the asynchronous removal operation. The task result contains a value that
    ///     indicates whether the module is successfully removed.
    /// </returns>
    public Task<bool> RemoveModuleAsync<T>() => RemoveModuleAsync(typeof(T));

    /// <summary>
    ///     Removes the command module.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of the module.</param>
    /// <returns>
    ///     A task that represents the asynchronous removal operation. The task result contains a value that
    ///     indicates whether the module is successfully removed.
    /// </returns>
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
    ///     Adds a custom <see cref="TypeReader" /> to this <see cref="CommandService" /> for the supplied object
    ///     type.
    ///     If <typeparamref name="T" /> is a <see cref="ValueType" />, a nullable <see cref="TypeReader" /> will
    ///     also be added.
    ///     If a default <see cref="TypeReader" /> exists for <typeparamref name="T" />, a warning will be logged
    ///     and the default <see cref="TypeReader" /> will be replaced.
    /// </summary>
    /// <typeparam name="T">The object type to be read by the <see cref="TypeReader"/>.</typeparam>
    /// <param name="reader">An instance of the <see cref="TypeReader" /> to be added.</param>
    public void AddTypeReader<T>(TypeReader reader) => AddTypeReader(typeof(T), reader);

    /// <summary>
    ///     Adds a custom <see cref="TypeReader" /> to this <see cref="CommandService" /> for the supplied object
    ///     type.
    ///     If <paramref name="type" /> is a <see cref="ValueType" />, a nullable <see cref="TypeReader" /> for the
    ///     value type will also be added.
    ///     If a default <see cref="TypeReader" /> exists for <paramref name="type" />, a warning will be logged and
    ///     the default <see cref="TypeReader" /> will be replaced.
    /// </summary>
    /// <param name="type">A <see cref="Type" /> instance for the type to be read.</param>
    /// <param name="reader">An instance of the <see cref="TypeReader" /> to be added.</param>
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
    ///     Adds a custom <see cref="TypeReader" /> to this <see cref="CommandService" /> for the supplied object
    ///     type.
    ///     If <typeparamref name="T" /> is a <see cref="ValueType" />, a nullable <see cref="TypeReader" /> will
    ///     also be added.
    /// </summary>
    /// <typeparam name="T">The object type to be read by the <see cref="TypeReader"/>.</typeparam>
    /// <param name="reader">An instance of the <see cref="TypeReader" /> to be added.</param>
    /// <param name="replaceDefault">
    ///     Defines whether the <see cref="TypeReader"/> should replace the default one for
    ///     <see cref="Type" /> if it exists.
    /// </param>
    public void AddTypeReader<T>(TypeReader reader, bool replaceDefault) =>
        AddTypeReader(typeof(T), reader, replaceDefault);

    /// <summary>
    ///     Adds a custom <see cref="TypeReader" /> to this <see cref="CommandService" /> for the supplied object
    ///     type.
    ///     If <paramref name="type" /> is a <see cref="ValueType" />, a nullable <see cref="TypeReader" /> for the
    ///     value type will also be added.
    /// </summary>
    /// <param name="type">A <see cref="Type" /> instance for the type to be read.</param>
    /// <param name="reader">An instance of the <see cref="TypeReader" /> to be added.</param>
    /// <param name="replaceDefault">
    ///     Defines whether the <see cref="TypeReader"/> should replace the default one for <see cref="Type" /> if
    ///     it exists.
    /// </param>
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
    ///     Removes a type reader from the list of type readers.
    /// </summary>
    /// <remarks>
    ///     Removing a <see cref="TypeReader"/> from the <see cref="CommandService"/> will not dereference the <see cref="TypeReader"/> from the loaded module/command instances.
    ///     You need to reload the modules for the changes to take effect.
    /// </remarks>
    /// <param name="type">The type to remove the readers from.</param>
    /// <param name="isDefaultTypeReader"><c>true</c> if the default readers for <paramref name="type"/> should be removed; otherwise <c>false</c>.</param>
    /// <param name="readers">The removed collection of type readers.</param>
    /// <returns><c>true</c> if the remove operation was successful; otherwise <c>false</c>.</returns>
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
    ///     Searches for the command.
    /// </summary>
    /// <param name="context">The context of the command.</param>
    /// <param name="argPos">The position of which the command starts at.</param>
    /// <returns>The result containing the matching commands.</returns>
    public SearchResult Search(ICommandContext context, int argPos) =>
        Search(context.Message.Content.Substring(argPos));

    /// <summary>
    ///     Searches for the command.
    /// </summary>
    /// <param name="context">The context of the command.</param>
    /// <param name="input">The command string.</param>
    /// <returns>The result containing the matching commands.</returns>
    public SearchResult Search(ICommandContext context, string input) => Search(input);

    /// <summary>
    ///     Searches for the command.
    /// </summary>
    /// <param name="input"> The command string. </param>
    /// <returns> The result containing the matching commands. </returns>
    public SearchResult Search(string input)
    {
        string searchInput = _caseSensitive ? input : input.ToLowerInvariant();
        ImmutableArray<CommandMatch> matches = [.._map.GetCommands(searchInput).OrderByDescending(x => x.Command.Priority)];

        return matches.Length > 0
            ? SearchResult.FromSuccess(input, matches)
            : SearchResult.FromError(CommandError.UnknownCommand, "Unknown command.");
    }

    /// <summary>
    ///     Executes the command.
    /// </summary>
    /// <param name="context">The context of the command.</param>
    /// <param name="argPos">The position of which the command starts at.</param>
    /// <param name="services">The service to be used in the command's dependency injection.</param>
    /// <param name="multiMatchHandling">The handling mode when multiple command matches are found.</param>
    /// <returns>
    ///     A task that represents the asynchronous execution operation. The task result contains the result of the
    ///     command execution.
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
    ///     Executes the command.
    /// </summary>
    /// <param name="context">The context of the command.</param>
    /// <param name="input">The command string.</param>
    /// <param name="services">The service to be used in the command's dependency injection.</param>
    /// <param name="multiMatchHandling">The handling mode when multiple command matches are found.</param>
    /// <returns>
    ///     A task that represents the asynchronous execution operation. The task result contains the result of the
    ///     command execution.
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
    /// Validates and gets the best <see cref="CommandMatch"/> from a specified <see cref="SearchResult"/>
    /// </summary>
    /// <param name="matches">The SearchResult.</param>
    /// <param name="context">The context of the command.</param>
    /// <param name="provider">The service provider to be used on the command's dependency injection.</param>
    /// <param name="multiMatchHandling">The handling mode when multiple command matches are found.</param>
    /// <returns>A task that represents the asynchronous validation operation. The task result contains the result of the
    ///     command validation as a <see cref="MatchResult"/> or a <see cref="SearchResult"/> if no matches were found.</returns>
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
