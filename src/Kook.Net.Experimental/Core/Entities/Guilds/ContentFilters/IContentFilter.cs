namespace Kook;

/// <summary>
///     表示一个服务器内容过滤器。
/// </summary>
public interface IContentFilter : IEntity<string>, IDeletable
{
    /// <summary>
    ///     获取此行为限制所属的服务器的 ID。
    /// </summary>
    ulong GuildId { get; }

    /// <summary>
    ///     获取此限制的名称。
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     获取此内容过滤器的目标。
    /// </summary>
    IContentFilterTarget Target { get; }

    /// <summary>
    ///     获取此内容过滤器的豁免列表。
    /// </summary>
    IReadOnlyCollection<ContentFilterExemption> Exemptions { get; }

    /// <summary>
    ///     获取此内容过滤器的处理程序。
    /// </summary>
    IReadOnlyCollection<IContentFilterHandler> Handlers { get; }

    /// <summary>
    ///     获取此行为限制是否启用。
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    ///     获取规则创建的时间。
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    ///     获取规则更新的时间。
    /// </summary>
    DateTimeOffset UpdatedAt { get; }
}
