using System.Diagnostics;

namespace Kook;

/// <summary>
///     表示一个通用的帖子话题标签。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly record struct ThreadTag : IEntity<uint>
{
    /// <inheritdoc/>
    public uint Id { get; }

    /// <summary>
    ///     获取话题标签的名称。
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     获取话题标签图标的链接。
    /// </summary>
    public string? Icon { get; }

    internal ThreadTag(uint id, string name, string? icon = null)
    {
        Id = id;
        Name = name;
        Icon = icon;
    }

    private string DebuggerDisplay => $"{nameof(ThreadTag)}: {Name} ({Id})";
}
