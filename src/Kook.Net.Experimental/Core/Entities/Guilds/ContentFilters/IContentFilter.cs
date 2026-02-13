namespace Kook;

/// <summary>
///     表示一个服务器内容过滤器。
/// </summary>
public interface IContentFilter : IEntity<string>, IDeletable
{
    /// <summary>
    ///     获取此内容过滤器所属的服务器的 ID。
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
    ///     获取此内容过滤器是否启用。
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    ///     获取内容过滤器创建的时间。
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    ///     获取内容过滤器更新的时间。
    /// </summary>
    DateTimeOffset UpdatedAt { get; }

    /// <summary>
    ///     修改服务器的内容过滤器信息。
    /// </summary>
    /// <param name="func"> 用于修改内容过滤器属性的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。 </returns>
    Task ModifyAsync(Action<ModifyContentFilterProperties> func, RequestOptions? options = null);

    /// <summary>
    ///     启用此内容过滤器。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。 </returns>
    Task EnableAsync(RequestOptions? options = null);

    /// <summary>
    ///     禁用此内容过滤器。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。 </returns>
    Task DisableAsync(RequestOptions? options = null);
}
