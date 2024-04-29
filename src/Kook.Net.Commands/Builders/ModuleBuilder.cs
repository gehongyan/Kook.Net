using System.Reflection;

namespace Kook.Commands.Builders;

/// <summary>
///     Represents a module builder.
/// </summary>
public class ModuleBuilder
{
    #region ModuleBuilder

    private string? _group;
    private readonly List<CommandBuilder> _commands;
    private readonly List<ModuleBuilder> _submodules;
    private readonly List<PreconditionAttribute> _preconditions;
    private readonly List<Attribute> _attributes;
    private readonly List<string> _aliases;

    /// <summary>
    ///     Gets the command service that this module builder belongs to.
    /// </summary>
    public CommandService Service { get; }

    /// <summary>
    ///     Gets the parent module builder that this module builder belongs to.
    /// </summary>
    public ModuleBuilder? Parent { get; }

    /// <summary>
    ///     Gets or sets the name of this module.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets the summary of this module.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    ///     Gets or sets the remarks of this module.
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    ///     Gets or sets the group of this module.
    /// </summary>
    public string? Group
    {
        get => _group;
        set
        {
            if (_group is not null)
                _aliases.Remove(_group);
            _group = value;
            AddAliases(value);
        }
    }

    /// <summary>
    ///     Gets a read-only list of commands that this module builder contains.
    /// </summary>
    public IReadOnlyList<CommandBuilder> Commands => _commands;

    /// <summary>
    ///     Gets a read-only list of submodules that this module builder contains.
    /// </summary>
    public IReadOnlyList<ModuleBuilder> Modules => _submodules;

    /// <summary>
    ///     Gets a read-only list of preconditions that this module builder contains.
    /// </summary>
    public IReadOnlyList<PreconditionAttribute> Preconditions => _preconditions;

    /// <summary>
    ///     Gets a read-only list of attributes that this module builder contains.
    /// </summary>
    public IReadOnlyList<Attribute> Attributes => _attributes;

    /// <summary>
    ///     Gets a read-only list of aliases that this module builder contains.
    /// </summary>
    public IReadOnlyList<string?> Aliases => _aliases;

    internal TypeInfo? TypeInfo { get; set; }

    #endregion

    #region Automatic

    /// <summary>
    ///     Initializes a new instance of the <see cref="ModuleBuilder"/> class.
    /// </summary>
    /// <param name="service"> The command service that this module builder belongs to. </param>
    /// <param name="parent"> The parent module builder that this module builder belongs to. </param>
    internal ModuleBuilder(CommandService service, ModuleBuilder? parent)
    {
        Service = service;
        Parent = parent;

        _commands = [];
        _submodules = [];
        _preconditions = [];
        _attributes = [];
        _aliases = [];
    }

    #endregion

    #region User-defined

    /// <summary>
    ///     Initializes a new instance of the <see cref="ModuleBuilder"/> class.
    /// </summary>
    /// <param name="service"> The command service that this module builder belongs to. </param>
    /// <param name="parent"> The parent module builder that this module builder belongs to. </param>
    /// <param name="primaryAlias"> The primary alias of this module. </param>
    internal ModuleBuilder(CommandService service, ModuleBuilder? parent, string primaryAlias)
        : this(service, parent)
    {
        Kook.Preconditions.NotNull(primaryAlias, nameof(primaryAlias));
        _aliases = [primaryAlias];
    }

    /// <summary>
    ///     Sets the name of this module.
    /// </summary>
    /// <param name="name"> The name of this module. </param>
    /// <returns> This module builder. </returns>
    public ModuleBuilder WithName(string name)
    {
        Name = name;
        return this;
    }

    /// <summary>
    ///     Sets the summary of this module.
    /// </summary>
    /// <param name="summary"> The summary of this module. </param>
    /// <returns> This module builder. </returns>
    public ModuleBuilder WithSummary(string summary)
    {
        Summary = summary;
        return this;
    }

    /// <summary>
    ///     Sets the remarks of this module.
    /// </summary>
    /// <param name="remarks"> The remarks of this module. </param>
    /// <returns> This module builder. </returns>
    public ModuleBuilder WithRemarks(string remarks)
    {
        Remarks = remarks;
        return this;
    }

    /// <summary>
    ///     Adds aliases to this module.
    /// </summary>
    /// <param name="aliases"> An array of aliases to add to this module. </param>
    /// <returns> This module builder. </returns>
    public ModuleBuilder AddAliases(params string?[] aliases)
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
    ///     Adds a precondition to this module.
    /// </summary>
    /// <param name="attributes"> An array of attributes to add to this module. </param>
    /// <returns> This module builder. </returns>
    public ModuleBuilder AddAttributes(params Attribute[] attributes)
    {
        _attributes.AddRange(attributes);
        return this;
    }

    /// <summary>
    ///     Adds a precondition to this module.
    /// </summary>
    /// <param name="precondition"> The precondition to add to this module. </param>
    /// <returns> This module builder. </returns>
    public ModuleBuilder AddPrecondition(PreconditionAttribute precondition)
    {
        _preconditions.Add(precondition);
        return this;
    }

    /// <summary>
    ///     Adds a command to this module.
    /// </summary>
    /// <param name="primaryAlias"> The primary alias of this command. </param>
    /// <param name="callback"> The callback of this command. </param>
    /// <param name="createFunc"> The function delegate that creates this command. </param>
    /// <returns> This module builder. </returns>
    public ModuleBuilder AddCommand(string primaryAlias,
        Func<ICommandContext, object?[], IServiceProvider, CommandInfo, Task> callback,
        Action<CommandBuilder> createFunc)
    {
        CommandBuilder builder = new(this, primaryAlias, callback);
        createFunc(builder);
        _commands.Add(builder);
        return this;
    }

    /// <summary>
    ///     Adds a command to this module.
    /// </summary>
    /// <param name="createFunc"> The function delegate that creates this command. </param>
    /// <returns> This module builder. </returns>
    internal ModuleBuilder AddCommand(Action<CommandBuilder> createFunc)
    {
        CommandBuilder builder = new(this);
        createFunc(builder);
        _commands.Add(builder);
        return this;
    }

    /// <summary>
    ///     Adds a module to this module.
    /// </summary>
    /// <param name="primaryAlias"> The primary alias of this module. </param>
    /// <param name="createFunc"> The function delegate that creates this module. </param>
    /// <returns> This module builder. </returns>
    public ModuleBuilder AddModule(string primaryAlias, Action<ModuleBuilder> createFunc)
    {
        ModuleBuilder builder = new(Service, this, primaryAlias);
        createFunc(builder);
        _submodules.Add(builder);
        return this;
    }

    /// <summary>
    ///     Adds a module to this module.
    /// </summary>
    /// <param name="createFunc"> The function delegate that creates this module. </param>
    /// <returns> This module builder. </returns>
    internal ModuleBuilder AddModule(Action<ModuleBuilder> createFunc)
    {
        ModuleBuilder builder = new(Service, this);
        createFunc(builder);
        _submodules.Add(builder);
        return this;
    }

    /// <summary>
    ///     Builds this module builder into a module.
    /// </summary>
    /// <param name="service"> The command service that this module builder belongs to. </param>
    /// <param name="services"> The service provider that this module builder belongs to. </param>
    /// <param name="parent"> The parent module that this module builder belongs to. </param>
    /// <returns> The built module. </returns>
    private ModuleInfo BuildImpl(CommandService service, IServiceProvider services, ModuleInfo? parent = null)
    {
        //Default name to first alias
        Name ??= _aliases[0];

        if (TypeInfo is { IsAbstract: false })
        {
            IModuleBase moduleInstance = ReflectionUtils.CreateObject<IModuleBase>(TypeInfo, service, services);
            moduleInstance.OnModuleBuilding(service, this);
        }

        return new ModuleInfo(this, service, services, parent);
    }

    /// <summary>
    ///     Builds this module builder into a module.
    /// </summary>
    /// <param name="service"> The command service that this module builder belongs to. </param>
    /// <param name="services"> The service provider that this module builder belongs to. </param>
    /// <returns> The built module. </returns>
    public ModuleInfo Build(CommandService service, IServiceProvider services) => BuildImpl(service, services);

    /// <summary>
    ///     Builds this module builder into a module.
    /// </summary>
    /// <param name="service"> The command service that this module builder belongs to. </param>
    /// <param name="services"> The service provider that this module builder belongs to. </param>
    /// <param name="parent"> The parent module that this module builder belongs to. </param>
    /// <returns> The built module. </returns>
    internal ModuleInfo Build(CommandService service, IServiceProvider services, ModuleInfo parent) => BuildImpl(service, services, parent);

    #endregion
}
