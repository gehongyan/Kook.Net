using Kook.Commands.Builders;

namespace Kook.Commands;

/// <summary>
///     表示一个模块的信息。
/// </summary>
public class ModuleInfo
{
    /// <summary>
    ///     获取与此模块关联的命令服务。
    /// </summary>
    public CommandService Service { get; }

    /// <summary>
    ///     获取此模块的名称。
    /// </summary>
    public string? Name { get; }

    /// <summary>
    ///     获取此模块的摘要。
    /// </summary>
    public string? Summary { get; }

    /// <summary>
    ///     获取此模块的备注。
    /// </summary>
    public string? Remarks { get; }

    /// <summary>
    ///     获取此模块的分组。
    /// </summary>
    public string? Group { get; }

    /// <summary>
    ///     获取此模块的所有别名。
    /// </summary>
    public IReadOnlyList<string> Aliases { get; }

    /// <summary>
    ///     获取此模块的所有命令。
    /// </summary>
    public IReadOnlyList<CommandInfo> Commands { get; }

    /// <summary>
    ///     获取此模块的所有先决条件。
    /// </summary>
    public IReadOnlyList<PreconditionAttribute> Preconditions { get; }

    /// <summary>
    ///     获取此模块的所有特性。
    /// </summary>
    public IReadOnlyList<Attribute> Attributes { get; }

    /// <summary>
    ///     获取此模块的所有子模块。
    /// </summary>
    public IReadOnlyList<ModuleInfo> Submodules { get; }

    /// <summary>
    ///     获取此模块的父模块。
    /// </summary>
    public ModuleInfo? Parent { get; }

    /// <summary>
    ///     获取此模块是否为子模块。
    /// </summary>
    public bool IsSubmodule => Parent != null;

    internal ModuleInfo(ModuleBuilder builder, CommandService service, IServiceProvider services, ModuleInfo? parent = null)
    {
        Service = service;

        Name = builder.Name;
        Summary = builder.Summary;
        Remarks = builder.Remarks;
        Group = builder.Group;
        Parent = parent;

        Aliases = [..BuildAliases(builder, service)];
        Commands = [..builder.Commands.Select(x => x.Build(this, service))];
        Preconditions = [..BuildPreconditions(builder)];
        Attributes = [..BuildAttributes(builder)];
        Submodules = [..BuildSubmodules(builder, service, services)];
    }

    private static IEnumerable<string> BuildAliases(ModuleBuilder builder, CommandService service)
    {
        List<string?> result = builder.Aliases.ToList();
        Queue<ModuleBuilder> builderQueue = new();

        ModuleBuilder? parent = builder;
        while ((parent = parent.Parent) != null)
            builderQueue.Enqueue(parent);

        while (builderQueue.Count > 0)
        {
            ModuleBuilder level = builderQueue.Dequeue();
            // permute in reverse because we want to *prefix* our aliases
            result = level.Aliases.Permutate(result, (first, second) =>
            {
                if (first == string.Empty) return second;
                if (second == string.Empty) return first;
                return first + service._separatorChar + second;
            }).ToList();
        }

        return result.OfType<string>().Distinct();
    }

    private List<ModuleInfo> BuildSubmodules(ModuleBuilder parent, CommandService service, IServiceProvider services) =>
        parent.Modules.Select(submodule => submodule.Build(service, services, this)).ToList();

    private static List<PreconditionAttribute> BuildPreconditions(ModuleBuilder builder)
    {
        List<PreconditionAttribute> result = [];

        ModuleBuilder? parent = builder;
        while (parent != null)
        {
            result.AddRange(parent.Preconditions);
            parent = parent.Parent;
        }

        return result;
    }

    private static List<Attribute> BuildAttributes(ModuleBuilder builder)
    {
        List<Attribute> result = [];

        ModuleBuilder? parent = builder;
        while (parent != null)
        {
            result.AddRange(parent.Attributes);
            parent = parent.Parent;
        }

        return result;
    }
}
