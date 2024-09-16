namespace Kook.Commands.Builders;

/// <summary>
///     表示一个命令构建器。
/// </summary>
public class CommandBuilder
{
    #region CommandBuilder

    private readonly List<PreconditionAttribute> _preconditions;
    private readonly List<ParameterBuilder> _parameters;
    private readonly List<Attribute> _attributes;
    private readonly List<string> _aliases;

    /// <summary>
    ///     获取此命令构建器所属的模块构建器。
    /// </summary>
    public ModuleBuilder Module { get; }

    /// <summary>
    ///     获取或设置当执行此命令时调用的回调。
    /// </summary>
    internal Func<ICommandContext, object?[], IServiceProvider, CommandInfo, Task>? Callback { get; set; }

    /// <summary>
    ///     获取或设置此命令的基本名称。
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     获取或设置此命令的摘要。
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    ///     获取或设置此命令的备注。
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    ///     获取或设置此命令的首要别名。
    /// </summary>
    public string? PrimaryAlias { get; set; }

    /// <summary>
    ///     获取或设置此命令的运行模式。
    /// </summary>
    public RunMode RunMode { get; set; }

    /// <summary>
    ///     获取或设置此命令的优先级。
    /// </summary>
    /// <seealso cref="Kook.Commands.PriorityAttribute(System.Int32)"/>
    public int Priority { get; set; }

    /// <summary>
    ///     获取或设置此命令是否忽略额外的参数。
    /// </summary>
    public bool IgnoreExtraArgs { get; set; }

    /// <summary>
    ///     获取此命令的先决条件。
    /// </summary>
    public IReadOnlyList<PreconditionAttribute> Preconditions => _preconditions;

    /// <summary>
    ///     获取此命令的参数构建器。
    /// </summary>
    public IReadOnlyList<ParameterBuilder> Parameters => _parameters;

    /// <summary>
    ///     获取此命令的特性。
    /// </summary>
    public IReadOnlyList<Attribute> Attributes => _attributes;

    /// <summary>
    ///     获取此命令的别名。
    /// </summary>
    public IReadOnlyList<string> Aliases => _aliases;

    #endregion

    #region Automatic

    internal CommandBuilder(ModuleBuilder module)
    {
        Module = module;
        _preconditions = [];
        _parameters = [];
        _attributes = [];
        _aliases = [];
    }

    #endregion

    #region User-defined

    /// <summary>
    ///     初始化一个 <see cref="CommandBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="module"> 此命令构建器所属的模块构建器。 </param>
    /// <param name="primaryAlias"> 此命令的首要别名。 </param>
    /// <param name="callback"> 当执行此命令时调用的回调。 </param>
    internal CommandBuilder(ModuleBuilder module, string? primaryAlias,
        Func<ICommandContext, object?[], IServiceProvider, CommandInfo, Task> callback)
        : this(module)
    {
        Kook.Preconditions.NotNull(primaryAlias, nameof(primaryAlias));
        Kook.Preconditions.NotNull(callback, nameof(callback));

        Callback = callback;
        PrimaryAlias = primaryAlias;
        _aliases.Add(primaryAlias);
    }

    /// <summary>
    ///     设置此命令的基本名称。
    /// </summary>
    /// <param name="name"> 此命令的基本名称。 </param>
    /// <returns> 当前命令构建器。 </returns>
    public CommandBuilder WithName(string name)
    {
        Name = name;
        return this;
    }

    /// <summary>
    ///     设置此命令的摘要。
    /// </summary>
    /// <param name="summary"> 此命令的摘要。 </param>
    /// <returns> 当前命令构建器。 </returns>
    public CommandBuilder WithSummary(string summary)
    {
        Summary = summary;
        return this;
    }

    /// <summary>
    ///      设置此命令的备注。
    /// </summary>
    /// <param name="remarks"> 此命令的备注。 </param>
    /// <returns> 当前命令构建器。 </returns>
    public CommandBuilder WithRemarks(string remarks)
    {
        Remarks = remarks;
        return this;
    }

    /// <summary>
    ///     设置此命令的运行模式。
    /// </summary>
    /// <param name="runMode"> 此命令的运行模式。 </param>
    /// <returns> 当前命令构建器。 </returns>
    public CommandBuilder WithRunMode(RunMode runMode)
    {
        RunMode = runMode;
        return this;
    }

    /// <summary>
    ///      设置此命令的优先级。
    /// </summary>
    /// <param name="priority"> 此命令的优先级。 </param>
    /// <returns> 当前命令构建器。 </returns>
    /// <seealso cref="Kook.Commands.PriorityAttribute(System.Int32)"/>
    public CommandBuilder WithPriority(int priority)
    {
        Priority = priority;
        return this;
    }

    /// <summary>
    ///     向此命令添加别名。
    /// </summary>
    /// <param name="aliases"> 包含要添加的别名的数组。 </param>
    /// <returns> 当前命令构建器。 </returns>
    public CommandBuilder AddAliases(params string?[] aliases)
    {
        foreach (string? x in aliases)
        {
            string alias = x ?? string.Empty;
            if (!_aliases.Contains(alias))
                _aliases.Add(alias);
        }

        return this;
    }

    /// <summary>
    ///     添加特性到此命令。
    /// </summary>
    /// <param name="attributes"> 要添加的特性。 </param>
    /// <returns> 当前命令构建器。 </returns>
    public CommandBuilder AddAttributes(params Attribute[] attributes)
    {
        _attributes.AddRange(attributes);
        return this;
    }

    /// <summary>
    ///     添加先决条件到此命令。
    /// </summary>
    /// <param name="precondition"> 要添加的先决条件。 </param>
    /// <returns> 当前命令构建器。 </returns>
    public CommandBuilder AddPrecondition(PreconditionAttribute precondition)
    {
        _preconditions.Add(precondition);
        return this;
    }

    /// <summary>
    ///     添加参数到此命令。
    /// </summary>
    /// <param name="name"> 参数的名称。 </param>
    /// <param name="createFunc"> 一个创建参数构建器的委托。 </param>
    /// <typeparam name="T"> 参数的类型。 </typeparam>
    /// <returns> 当前命令构建器。 </returns>
    public CommandBuilder AddParameter<T>(string name, Action<ParameterBuilder> createFunc)
    {
        ParameterBuilder param = new(this, name, typeof(T));
        createFunc(param);
        _parameters.Add(param);
        return this;
    }

    /// <summary>
    ///     添加参数到此命令。
    /// </summary>
    /// <param name="name"> 参数的名称。 </param>
    /// <param name="type"> 参数的类型。 </param>
    /// <param name="createFunc"> 一个创建参数构建器的委托。 </param>
    /// <returns> 当前命令构建器。 </returns>
    public CommandBuilder AddParameter(string name, Type type, Action<ParameterBuilder> createFunc)
    {
        ParameterBuilder param = new(this, name, type);
        createFunc(param);
        _parameters.Add(param);
        return this;
    }

    /// <summary>
    ///     添加参数到此命令。
    /// </summary>
    /// <param name="createFunc"> 一个创建参数构建器的委托。 </param>
    /// <returns> 当前命令构建器。 </returns>
    internal CommandBuilder AddParameter(Action<ParameterBuilder> createFunc)
    {
        ParameterBuilder param = new(this);
        createFunc(param);
        _parameters.Add(param);
        return this;
    }

    /// <summary>
    ///     构建此命令构建器。
    /// </summary>
    /// <param name="info"> 此命令所属的模块信息。 </param>
    /// <param name="service"> 此命令所属的命令服务。 </param>
    /// <returns> 此命令构建器构建的命令信息。 </returns>
    /// <exception cref="InvalidOperationException"> 仅支持在最后一个参数上设置接收全部剩余参数或接收多个参数。 </exception>
    internal CommandInfo Build(ModuleInfo info, CommandService service)
    {
        // Default name to primary alias
        Name ??= PrimaryAlias;

        if (_parameters.Count > 0)
        {
            ParameterBuilder lastParam = _parameters[^1];

            ParameterBuilder? firstMultipleParam = _parameters.Find(x => x.IsMultiple);
            if (firstMultipleParam != null && firstMultipleParam != lastParam)
            {
                throw new InvalidOperationException(
                    $"Only the last parameter in a command may have the Multiple flag. Parameter: {firstMultipleParam.Name} in {PrimaryAlias}");
            }

            ParameterBuilder? firstRemainderParam = _parameters.Find(x => x.IsRemainder);
            if (firstRemainderParam != null && firstRemainderParam != lastParam)
            {
                throw new InvalidOperationException(
                    $"Only the last parameter in a command may have the Remainder flag. Parameter: {firstRemainderParam.Name} in {PrimaryAlias}");
            }
        }

        return new CommandInfo(this, info, service);
    }

    #endregion
}
