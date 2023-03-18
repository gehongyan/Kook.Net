namespace Kook.Commands.Builders;

/// <summary>
///     Represents a command builder.
/// </summary>
public class CommandBuilder
{
    #region CommandBuilder

    private readonly List<PreconditionAttribute> _preconditions;
    private readonly List<ParameterBuilder> _parameters;
    private readonly List<Attribute> _attributes;
    private readonly List<string> _aliases;

    /// <summary>
    ///     Gets the module builder that this command builder belongs to.
    /// </summary>
    public ModuleBuilder Module { get; }

    /// <summary>
    ///     Gets or sets the callback that is invoked when this command is executed.
    /// </summary>
    internal Func<ICommandContext, object[], IServiceProvider, CommandInfo, Task> Callback { get; set; }

    /// <summary>
    ///     Gets or sets the name of this command.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Gets or sets the summary of this command.
    /// </summary>
    public string Summary { get; set; }

    /// <summary>
    ///     Gets or sets the remarks of this command.
    /// </summary>
    public string Remarks { get; set; }

    /// <summary>
    ///     Gets or sets the primary alias of this command.
    /// </summary>
    public string PrimaryAlias { get; set; }

    /// <summary>
    ///     Gets or sets the run mode of this command.
    /// </summary>
    public RunMode RunMode { get; set; }

    /// <summary>
    ///     Gets or sets the priority of this command.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    ///     Gets or sets whether the extra arguments should be ignored.
    /// </summary>
    public bool IgnoreExtraArgs { get; set; }

    /// <summary>
    ///     Gets the preconditions of this command.
    /// </summary>
    public IReadOnlyList<PreconditionAttribute> Preconditions => _preconditions;

    /// <summary>
    ///     Gets the parameters of this command.
    /// </summary>
    public IReadOnlyList<ParameterBuilder> Parameters => _parameters;

    /// <summary>
    ///     Gets the attributes of this command.
    /// </summary>
    public IReadOnlyList<Attribute> Attributes => _attributes;

    /// <summary>
    ///     Gets the aliases of this command.
    /// </summary>
    public IReadOnlyList<string> Aliases => _aliases;

    #endregion

    #region Automatic

    /// <summary>
    ///     Initializes a new instance of the <see cref="CommandBuilder"/> class.
    /// </summary>
    /// <param name="module"> The module builder that this command builder belongs to. </param>
    internal CommandBuilder(ModuleBuilder module)
    {
        Module = module;

        _preconditions = new List<PreconditionAttribute>();
        _parameters = new List<ParameterBuilder>();
        _attributes = new List<Attribute>();
        _aliases = new List<string>();
    }

    #endregion

    #region User-defined

    /// <summary>
    ///     Initializes a new instance of the <see cref="CommandBuilder"/> class.
    /// </summary>
    /// <param name="module"> The module builder that this command builder belongs to. </param>
    /// <param name="primaryAlias"> The primary alias of this command. </param>
    /// <param name="callback"> The callback that is invoked when this command is executed. </param>
    internal CommandBuilder(ModuleBuilder module, string primaryAlias, Func<ICommandContext, object[], IServiceProvider, CommandInfo, Task> callback)
        : this(module)
    {
        Kook.Preconditions.NotNull(primaryAlias, nameof(primaryAlias));
        Kook.Preconditions.NotNull(callback, nameof(callback));

        Callback = callback;
        PrimaryAlias = primaryAlias;
        _aliases.Add(primaryAlias);
    }

    /// <summary>
    ///     Sets the name of this command.
    /// </summary>
    /// <param name="name"> The name of this command. </param>
    /// <returns> This command builder. </returns>
    public CommandBuilder WithName(string name)
    {
        Name = name;
        return this;
    }

    /// <summary>
    ///     Sets the summary of this command.
    /// </summary>
    /// <param name="summary"> The summary of this command. </param>
    /// <returns> This command builder. </returns>
    public CommandBuilder WithSummary(string summary)
    {
        Summary = summary;
        return this;
    }

    /// <summary>
    ///      Sets the remarks of this command.
    /// </summary>
    /// <param name="remarks"> The remarks of this command. </param>
    /// <returns> This command builder. </returns>
    public CommandBuilder WithRemarks(string remarks)
    {
        Remarks = remarks;
        return this;
    }

    /// <summary>
    ///     Sets the run mode of this command.
    /// </summary>
    /// <param name="runMode"> The run mode of this command. </param>
    /// <returns> This command builder. </returns>
    public CommandBuilder WithRunMode(RunMode runMode)
    {
        RunMode = runMode;
        return this;
    }

    /// <summary>
    ///      Sets the priority of this command.
    /// </summary>
    /// <param name="priority"> The priority of this command. </param>
    /// <returns> This command builder. </returns>
    public CommandBuilder WithPriority(int priority)
    {
        Priority = priority;
        return this;
    }

    /// <summary>
    ///     Adds aliases to this command.
    /// </summary>
    /// <param name="aliases"> An array containing the aliases to add. </param>
    /// <returns> This command builder. </returns>
    public CommandBuilder AddAliases(params string[] aliases)
    {
        for (int i = 0; i < aliases.Length; i++)
        {
            string alias = aliases[i] ?? "";
            if (!_aliases.Contains(alias)) _aliases.Add(alias);
        }

        return this;
    }

    /// <summary>
    ///     Adds attributes to this command.
    /// </summary>
    /// <param name="attributes"> An array containing the attributes to add. </param>
    /// <returns> This command builder. </returns>
    public CommandBuilder AddAttributes(params Attribute[] attributes)
    {
        _attributes.AddRange(attributes);
        return this;
    }

    /// <summary>
    ///     Adds a precondition to this command.
    /// </summary>
    /// <param name="precondition"> The precondition to add. </param>
    /// <returns> This command builder. </returns>
    public CommandBuilder AddPrecondition(PreconditionAttribute precondition)
    {
        _preconditions.Add(precondition);
        return this;
    }

    /// <summary>
    ///     Adds a parameter to this command.
    /// </summary>
    /// <param name="name"> The name of the parameter. </param>
    /// <param name="createFunc"> An action delegate that is invoked to create the parameter. </param>
    /// <typeparam name="T"> The type of the parameter. </typeparam>
    /// <returns> This command builder. </returns>
    public CommandBuilder AddParameter<T>(string name, Action<ParameterBuilder> createFunc)
    {
        ParameterBuilder param = new(this, name, typeof(T));
        createFunc(param);
        _parameters.Add(param);
        return this;
    }

    /// <summary>
    ///     Adds a parameter to this command.
    /// </summary>
    /// <param name="name"> The name of the parameter. </param>
    /// <param name="type"> The type of the parameter. </param>
    /// <param name="createFunc"> An action delegate that is invoked to create the parameter. </param>
    /// <returns> This command builder. </returns>
    public CommandBuilder AddParameter(string name, Type type, Action<ParameterBuilder> createFunc)
    {
        ParameterBuilder param = new(this, name, type);
        createFunc(param);
        _parameters.Add(param);
        return this;
    }

    /// <summary>
    ///     Adds a parameter to this command.
    /// </summary>
    /// <param name="createFunc"> An action delegate that is invoked to create the parameter. </param>
    /// <returns> This command builder. </returns>
    internal CommandBuilder AddParameter(Action<ParameterBuilder> createFunc)
    {
        ParameterBuilder param = new(this);
        createFunc(param);
        _parameters.Add(param);
        return this;
    }

    /// <summary>
    ///     Builds the command.
    /// </summary>
    /// <param name="info"> The module info. </param>
    /// <param name="service"> The command service. </param>
    /// <returns> The command info. </returns>
    /// <exception cref="InvalidOperationException">Only the last parameter in a command may have the Remainder or Multiple flag.</exception>
    internal CommandInfo Build(ModuleInfo info, CommandService service)
    {
        // Default name to primary alias
        Name ??= PrimaryAlias;

        if (_parameters.Count > 0)
        {
            ParameterBuilder lastParam = _parameters[_parameters.Count - 1];

            ParameterBuilder firstMultipleParam = _parameters.FirstOrDefault(x => x.IsMultiple);
            if (firstMultipleParam != null && firstMultipleParam != lastParam)
                throw new InvalidOperationException(
                    $"Only the last parameter in a command may have the Multiple flag. Parameter: {firstMultipleParam.Name} in {PrimaryAlias}");

            ParameterBuilder firstRemainderParam = _parameters.FirstOrDefault(x => x.IsRemainder);
            if (firstRemainderParam != null && firstRemainderParam != lastParam)
                throw new InvalidOperationException(
                    $"Only the last parameter in a command may have the Remainder flag. Parameter: {firstRemainderParam.Name} in {PrimaryAlias}");
        }

        return new CommandInfo(this, info, service);
    }

    #endregion
}
