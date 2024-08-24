namespace Kook;

/// <summary>
///     表示一个通用的 POKE。
/// </summary>
public interface IPoke : IEntity<uint>
{
    /// <summary>
    ///     获取 POKE 的名称。
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     获取 POKE 的描述。
    /// </summary>
    string Description { get; }

    /// <summary>
    ///     获取用户使用此 POKE 后的冷却时间。
    /// </summary>
    TimeSpan Cooldown { get; }

    /// <summary>
    ///     获取此 POKE 的分类。
    /// </summary>
    IReadOnlyCollection<string> Categories { get; }

    /// <summary>
    ///     获取此 POKE 的标签。
    /// </summary>
    PokeLabel Label { get; }

    /// <summary>
    ///     获取此 POKE 的图标资源。
    /// </summary>
    PokeIcon Icon { get; }

    /// <summary>
    ///     获取此 POKE 的品质。
    /// </summary>
    PokeQuality Quality { get; }

    /// <summary>
    ///     获取此 POKE 的资源。
    /// </summary>
    IPokeResource Resource { get; }

    /// <summary>
    ///     获取此 POKE 如何在消息上下文中使用和显示。
    /// </summary>
    IReadOnlyDictionary<string, string> MessageScenarios { get; }
}
