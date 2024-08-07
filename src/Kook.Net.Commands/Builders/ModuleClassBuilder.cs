using Kook.Commands.Builders;
using System.Reflection;

namespace Kook.Commands;

internal static class ModuleClassBuilder
{
    private static readonly TypeInfo ModuleTypeInfo = typeof(IModuleBase).GetTypeInfo();

    public static async Task<IReadOnlyList<TypeInfo>> SearchAsync(Assembly assembly, CommandService service)
    {
        List<TypeInfo> result = [];

        foreach (TypeInfo typeInfo in assembly.DefinedTypes)
        {
            if (typeInfo.IsPublic || typeInfo.IsNestedPublic)
            {
                if (IsValidModuleDefinition(typeInfo) && !typeInfo.IsDefined(typeof(DontAutoLoadAttribute)))
                    result.Add(typeInfo);
            }
            else if (IsLoadableModule(typeInfo))
            {
                await service
                    ._cmdLogger
                    .WarningAsync($"Class {typeInfo.FullName} is not public and cannot be loaded. To suppress this message, mark the class with {nameof(DontAutoLoadAttribute)}.")
                    .ConfigureAwait(false);
            }
        }

        return result;
        bool IsLoadableModule(TypeInfo info) =>
            info.DeclaredMethods.Any(x => x.GetCustomAttribute<CommandAttribute>() != null)
            && info.GetCustomAttribute<DontAutoLoadAttribute>() == null;
    }

    public static Task<Dictionary<Type, ModuleInfo>> BuildAsync(CommandService service,
        IServiceProvider services, params TypeInfo[] validTypes) =>
        BuildAsync(validTypes, service, services);

    public static async Task<Dictionary<Type, ModuleInfo>> BuildAsync(IEnumerable<TypeInfo> validTypes,
        CommandService service, IServiceProvider services)
    {
        /*if (!validTypes.Any())
            throw new InvalidOperationException("Could not find any valid modules from the given selection");*/

        IEnumerable<TypeInfo> topLevelGroups = validTypes
            .Where(x => x.DeclaringType == null || !IsValidModuleDefinition(x.DeclaringType.GetTypeInfo()));
        List<TypeInfo> builtTypes = [];
        Dictionary<Type, ModuleInfo> result = [];

        foreach (TypeInfo typeInfo in topLevelGroups)
        {
            // TODO: This shouldn't be the case; may be safe to remove?
            if (result.ContainsKey(typeInfo.AsType()))
                continue;

            ModuleBuilder module = new(service, null);

            BuildModule(module, typeInfo, service, services);
            BuildSubTypes(module, typeInfo.DeclaredNestedTypes, builtTypes, service, services);
            builtTypes.Add(typeInfo);

            result[typeInfo.AsType()] = module.Build(service, services);
        }

        await service
            ._cmdLogger
            .DebugAsync($"Successfully built {builtTypes.Count} modules.")
            .ConfigureAwait(false);

        return result;
    }

    private static void BuildSubTypes(ModuleBuilder builder, IEnumerable<TypeInfo> subTypes, List<TypeInfo> builtTypes, CommandService service,
        IServiceProvider services)
    {
        foreach (TypeInfo typeInfo in subTypes)
        {
            if (!IsValidModuleDefinition(typeInfo))
                continue;
            if (builtTypes.Contains(typeInfo))
                continue;
            builder.AddModule((module) =>
            {
                BuildModule(module, typeInfo, service, services);
                BuildSubTypes(module, typeInfo.DeclaredNestedTypes, builtTypes, service, services);
            });

            builtTypes.Add(typeInfo);
        }
    }

    private static void BuildModule(ModuleBuilder builder, TypeInfo typeInfo, CommandService service, IServiceProvider services)
    {
        IEnumerable<Attribute> attributes = typeInfo.GetCustomAttributes();
        builder.TypeInfo = typeInfo;

        foreach (Attribute attribute in attributes)
        {
            switch (attribute)
            {
                case NameAttribute name:
                    builder.Name = name.Text;
                    break;
                case SummaryAttribute summary:
                    builder.Summary = summary.Text;
                    break;
                case RemarksAttribute remarks:
                    builder.Remarks = remarks.Text;
                    break;
                case AliasAttribute alias:
                    builder.AddAliases(alias.Aliases);
                    break;
                case GroupAttribute group:
                    builder.Name ??= group.Prefix;
                    builder.Group = group.Prefix;
                    break;
                case PreconditionAttribute precondition:
                    builder.AddPrecondition(precondition);
                    break;
                default:
                    builder.AddAttributes(attribute);
                    break;
            }
        }

        //Check for unspecified info
        if (builder.Aliases.Count == 0)
            builder.AddAliases(string.Empty);
        builder.Name ??= typeInfo.Name;

        // Get all methods (including from inherited members), that are valid commands
        IEnumerable<MethodInfo> validCommands = typeInfo
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(IsValidCommandDefinition);

        foreach (MethodInfo method in validCommands) builder.AddCommand((command) => { BuildCommand(command, typeInfo, method, service, services); });
    }

    private static void BuildCommand(CommandBuilder builder, TypeInfo typeInfo, MethodInfo method,
        CommandService service, IServiceProvider serviceprovider)
    {
        IEnumerable<Attribute> attributes = method.GetCustomAttributes();
        foreach (Attribute attribute in attributes)
        {
            switch (attribute)
            {
                case CommandAttribute command:
                    builder.Summary ??= command.Summary;
                    builder.Remarks ??= command.Remarks;
                    if (command.Aliases is { Length: > 0 })
                        builder.AddAliases(command.Aliases);
                    builder.AddAliases(command.Text);
                    builder.RunMode = command.RunMode;
                    builder.Name ??= command.Text;
                    builder.IgnoreExtraArgs = command.IgnoreExtraArgs ?? service._ignoreExtraArgs;
                    break;
                case NameAttribute name:
                    builder.Name = name.Text;
                    break;
                case PriorityAttribute priority:
                    builder.Priority = priority.Priority;
                    break;
                case SummaryAttribute summary:
                    builder.Summary = summary.Text;
                    break;
                case RemarksAttribute remarks:
                    builder.Remarks = remarks.Text;
                    break;
                case AliasAttribute alias:
                    builder.AddAliases(alias.Aliases);
                    break;
                case PreconditionAttribute precondition:
                    builder.AddPrecondition(precondition);
                    break;
                default:
                    builder.AddAttributes(attribute);
                    break;
            }
        }

        builder.Name ??= method.Name;
        System.Reflection.ParameterInfo[] parameters = method.GetParameters();
        int pos = 0;
        int count = parameters.Length;
        foreach (System.Reflection.ParameterInfo paramInfo in parameters)
            builder.AddParameter(parameter => BuildParameter(parameter, paramInfo, pos++, count, service, serviceprovider));

        Func<IServiceProvider, IModuleBase> createInstance = ReflectionUtils.CreateBuilder<IModuleBase>(typeInfo, service);
        builder.Callback = ExecuteCallback;
        return;

        async Task<IResult> ExecuteCallback(ICommandContext context, object?[] args, IServiceProvider services, CommandInfo cmd)
        {
            IModuleBase instance = createInstance(services);
            instance.SetContext(context);

            try
            {
                await instance.BeforeExecuteAsync(cmd).ConfigureAwait(false);
                // ReSharper disable once MethodHasAsyncOverload
                instance.BeforeExecute(cmd);

                Task task = method.Invoke(instance, args) as Task ?? Task.CompletedTask;
                if (task is Task<RuntimeResult> resultTask)
                    return await resultTask.ConfigureAwait(false);
                else
                {
                    await task.ConfigureAwait(false);
                    return ExecuteResult.FromSuccess();
                }
            }
            finally
            {
                await instance.AfterExecuteAsync(cmd).ConfigureAwait(false);
                // ReSharper disable once MethodHasAsyncOverload
                instance.AfterExecute(cmd);
                // ReSharper disable once SuspiciousTypeConversion.Global
                (instance as IDisposable)?.Dispose();
            }
        }
    }

    private static void BuildParameter(ParameterBuilder builder,
        System.Reflection.ParameterInfo paramInfo, int position, int count,
        CommandService service, IServiceProvider services)
    {
        IEnumerable<Attribute> attributes = paramInfo.GetCustomAttributes();
        Type? paramType = paramInfo.ParameterType;

        builder.Name = paramInfo.Name ?? string.Empty;
        builder.IsOptional = paramInfo.IsOptional;
        builder.DefaultValue = paramInfo.HasDefaultValue ? paramInfo.DefaultValue : null;

        foreach (Attribute attribute in attributes)
        {
            switch (attribute)
            {
                case SummaryAttribute summary:
                    builder.Summary = summary.Text;
                    break;
                case OverrideTypeReaderAttribute typeReader:
                    builder.TypeReader = GetTypeReader(service, paramType, typeReader.TypeReader, services);
                    break;
                case ParamArrayAttribute:
                    builder.IsMultiple = true;
                    paramType = paramType?.GetElementType();
                    break;
                case ParameterPreconditionAttribute precondition:
                    builder.AddPrecondition(precondition);
                    break;
                case NameAttribute name:
                    builder.Name = name.Text;
                    break;
                case RemainderAttribute:
                    if (position != count - 1)
                        throw new InvalidOperationException($"Remainder parameters must be the last parameter in a command. Parameter: {paramInfo.Name} in {paramInfo.Member.DeclaringType?.Name}.{paramInfo.Member.Name}");
                    builder.IsRemainder = true;
                    break;
                default:
                    builder.AddAttributes(attribute);
                    break;
            }
        }

        builder.ParameterType = paramType;
        builder.TypeReader ??= service.GetDefaultTypeReader(paramType)
            ?? service.GetTypeReaders(paramType)?.FirstOrDefault().Value;
    }

    internal static TypeReader GetTypeReader(CommandService service, Type? paramType, Type typeReaderType, IServiceProvider services)
    {
        IDictionary<Type, TypeReader>? readers = service.GetTypeReaders(paramType);
        if (readers != null && readers.TryGetValue(typeReaderType, out TypeReader? reader))
            return reader;

        //We don't have a cached type reader, create one
        reader = ReflectionUtils.CreateObject<TypeReader>(typeReaderType.GetTypeInfo(), service, services);
        if (paramType is not null)
            service.AddTypeReader(paramType, reader, false);
        return reader;
    }

    private static bool IsValidModuleDefinition(TypeInfo typeInfo) =>
        ModuleTypeInfo.IsAssignableFrom(typeInfo) && !typeInfo.IsAbstract && !typeInfo.ContainsGenericParameters;

    private static bool IsValidCommandDefinition(MethodInfo methodInfo) =>
        methodInfo.IsDefined(typeof(CommandAttribute))
        && (methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType == typeof(Task<RuntimeResult>))
        && !methodInfo.IsStatic
        && !methodInfo.IsGenericMethod;
}
