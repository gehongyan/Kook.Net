using System.Reflection;

namespace Kook.Commands.Builders;

/// <summary>
///     表示一个模块构建器。
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
    ///     获取此模块构建器所属的命令服务。
    /// </summary>
    public CommandService Service { get; }

    /// <summary>
    ///     获取此模块构建器所属的父模块构建器。
    /// </summary>
    public ModuleBuilder? Parent { get; }

    /// <summary>
    ///     获取或设置此模块的基本名称。
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     获取或设置此模块的摘要。
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    ///     获取或设置此模块的备注。
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    ///     获取或设置此模块的分组。
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
    ///     获取此模块的别名。
    /// </summary>
    public IReadOnlyList<CommandBuilder> Commands => _commands;

    /// <summary>
    ///     获取此模块的子模块。
    /// </summary>
    public IReadOnlyList<ModuleBuilder> Modules => _submodules;

    /// <summary>
    ///     获取此模块的先决条件。
    /// </summary>
    public IReadOnlyList<PreconditionAttribute> Preconditions => _preconditions;

    /// <summary>
    ///     获取此模块的特性。
    /// </summary>
    public IReadOnlyList<Attribute> Attributes => _attributes;

    /// <summary>
    ///     获取此模块的别名。
    /// </summary>
    public IReadOnlyList<string?> Aliases => _aliases;

    internal TypeInfo? TypeInfo { get; set; }

    #endregion

    #region Automatic

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
    ///     初始化一个 <see cref="ModuleBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="service"> 此模块构建器所属的命令服务。 </param>
    /// <param name="parent"> 此模块构建器所属的父模块构建器。 </param>
    /// <param name="primaryAlias"> 此模块的首要别名。 </param>
    internal ModuleBuilder(CommandService service, ModuleBuilder? parent, string primaryAlias)
        : this(service, parent)
    {
        Kook.Preconditions.NotNull(primaryAlias, nameof(primaryAlias));
        _aliases = [primaryAlias];
    }

    /// <summary>
    ///     设置此模块的名基本称。
    /// </summary>
    /// <param name="name"> 此模块的基本名称。 </param>
    /// <returns> 此模块构建器。 </returns>
    public ModuleBuilder WithName(string name)
    {
        Name = name;
        return this;
    }

    /// <summary>
    ///     设置此模块的摘要。
    /// </summary>
    /// <param name="summary"> 此模块的摘要。 </param>
    /// <returns> 此模块构建器。 </returns>
    public ModuleBuilder WithSummary(string summary)
    {
        Summary = summary;
        return this;
    }

    /// <summary>
    ///     设置此模块的备注。
    /// </summary>
    /// <param name="remarks"> 此模块的备注。 </param>
    /// <returns> 此模块构建器。 </returns>
    public ModuleBuilder WithRemarks(string remarks)
    {
        Remarks = remarks;
        return this;
    }

    /// <summary>
    ///     添加别名到此模块。
    /// </summary>
    /// <param name="aliases"> 要添加到此模块的别名数组。 </param>
    /// <returns> 此模块构建器。 </returns>
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
    ///     添加特性到此模块。
    /// </summary>
    /// <param name="attributes"> 要添加到此模块的特性数组。 </param>
    /// <returns> 此模块构建器。 </returns>
    public ModuleBuilder AddAttributes(params Attribute[] attributes)
    {
        _attributes.AddRange(attributes);
        return this;
    }

    /// <summary>
    ///     添加先决条件到此模块。
    /// </summary>
    /// <param name="precondition"> 要添加到此模块的先决条件。 </param>
    /// <returns> 此模块构建器。 </returns>
    public ModuleBuilder AddPrecondition(PreconditionAttribute precondition)
    {
        _preconditions.Add(precondition);
        return this;
    }

    /// <summary>
    ///     添加命令到此模块。
    /// </summary>
    /// <param name="primaryAlias"> 此命令的首要别名。 </param>
    /// <param name="callback"> 当执行此命令时调用的回调。 </param>
    /// <param name="createFunc"> 一个创建命令构建器的委托。 </param>
    /// <returns> 此模块构建器。 </returns>
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
    ///     添加命令到此模块。
    /// </summary>
    /// <param name="createFunc"> 一个创建命令构建器的委托。 </param>
    /// <returns> 此模块构建器。 </returns>
    internal ModuleBuilder AddCommand(Action<CommandBuilder> createFunc)
    {
        CommandBuilder builder = new(this);
        createFunc(builder);
        _commands.Add(builder);
        return this;
    }

    /// <summary>
    ///     添加子模块到此模块。
    /// </summary>
    /// <param name="primaryAlias"> 此模块的首要别名。 </param>
    /// <param name="createFunc"> 一个创建模块构建器的委托。 </param>
    /// <returns> 此模块构建器。 </returns>
    public ModuleBuilder AddModule(string primaryAlias, Action<ModuleBuilder> createFunc)
    {
        ModuleBuilder builder = new(Service, this, primaryAlias);
        createFunc(builder);
        _submodules.Add(builder);
        return this;
    }

    /// <summary>
    ///     添加子模块到此模块。
    /// </summary>
    /// <param name="createFunc"> 一个创建模块构建器的委托。 </param>
    /// <returns> 此模块构建器。 </returns>
    internal ModuleBuilder AddModule(Action<ModuleBuilder> createFunc)
    {
        ModuleBuilder builder = new(Service, this);
        createFunc(builder);
        _submodules.Add(builder);
        return this;
    }

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
    ///     构建此模块构建器为模块。
    /// </summary>
    /// <param name="service"> 此模块构建器所属的命令服务。 </param>
    /// <param name="services"> 此模块构建器所属的服务提供程序。 </param>
    /// <returns> 构建的模块。 </returns>
    public ModuleInfo Build(CommandService service, IServiceProvider services) => BuildImpl(service, services);

    /// <summary>
    ///     构建此模块构建器为模块。
    /// </summary>
    /// <param name="service"> 此模块构建器所属的命令服务。 </param>
    /// <param name="services"> 此模块构建器所属的服务提供程序。 </param>
    /// <param name="parent"> 此模块构建器所属的父模块。 </param>
    /// <returns> 构建的模块。 </returns>
    internal ModuleInfo Build(CommandService service, IServiceProvider services, ModuleInfo parent) => BuildImpl(service, services, parent);

    #endregion
}
