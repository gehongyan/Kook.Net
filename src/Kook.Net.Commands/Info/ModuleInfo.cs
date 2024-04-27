using Kook.Commands.Builders;
using System.Collections.Immutable;

namespace Kook.Commands;

/// <summary>
///     Provides the information of a module.
/// </summary>
public class ModuleInfo
{
    /// <summary>
    ///     Gets the command service associated with this module.
    /// </summary>
    public CommandService Service { get; }

    /// <summary>
    ///     Gets the name of this module.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    ///     Gets the summary of this module.
    /// </summary>
    public string? Summary { get; }

    /// <summary>
    ///     Gets the remarks of this module.
    /// </summary>
    public string? Remarks { get; }

    /// <summary>
    ///     Gets the group name (main prefix) of this module.
    /// </summary>
    public string? Group { get; }

    /// <summary>
    ///     Gets a read-only list of aliases associated with this module.
    /// </summary>
    public IReadOnlyList<string> Aliases { get; }

    /// <summary>
    ///     Gets a read-only list of commands associated with this module.
    /// </summary>
    public IReadOnlyList<CommandInfo> Commands { get; }

    /// <summary>
    ///     Gets a read-only list of preconditions that apply to this module.
    /// </summary>
    public IReadOnlyList<PreconditionAttribute> Preconditions { get; }

    /// <summary>
    ///     Gets a read-only list of attributes that apply to this module.
    /// </summary>
    public IReadOnlyList<Attribute> Attributes { get; }

    /// <summary>
    ///     Gets a read-only list of submodules associated with this module.
    /// </summary>
    public IReadOnlyList<ModuleInfo> Submodules { get; }

    /// <summary>
    ///     Gets the parent module of this submodule if applicable.
    /// </summary>
    public ModuleInfo? Parent { get; }

    /// <summary>
    ///     Gets a value that indicates whether this module is a submodule or not.
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
