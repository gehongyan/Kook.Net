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
    public string? Name { get; }

    /// <summary>
    ///     获取话题标签图标的链接。
    /// </summary>
    public string? Icon { get; }

    /// <summary>
    ///     初始化一个新的 <see cref="ThreadTag"/> 实例。
    /// </summary>
    /// <param name="id"> 话题标签的 ID。</param>
    public ThreadTag(uint id)
    {
        Id = id;
    }

    internal ThreadTag(uint id, string name, string? icon = null)
        : this(id)
    {
        Id = id;
        Name = name;
        Icon = icon;
    }

    /// <inheritdoc cref="Kook.ThreadTag(System.UInt32)" />
    public static implicit operator ThreadTag(uint id) => new(id);

    private string DebuggerDisplay => Name is not null ? $"{Name} ({Id})" : Id.ToString();
}
