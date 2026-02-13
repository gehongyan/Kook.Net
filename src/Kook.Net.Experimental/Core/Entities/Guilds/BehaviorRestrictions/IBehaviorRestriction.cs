namespace Kook;

/// <summary>
///     表示一个通用的服务器行为限制。
/// </summary>
public interface IBehaviorRestriction : IEntity<string>, IDeletable
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
    ///     获取限制条件类型。
    /// </summary>
    IReadOnlyCollection<IBehaviorRestrictionCondition> Conditions { get; }

    /// <summary>
    ///     获取限制持续时长。
    /// </summary>
    TimeSpan Duration { get; }

    /// <summary>
    ///     获取限制的行为类型。
    /// </summary>
    BehaviorRestrictionType Type { get; }

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

    /// <summary>
    ///     修改服务器的行为限制信息。
    /// </summary>
    /// <param name="func"> 用于修改行为限制属性的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。 </returns>
    Task ModifyAsync(Action<ModifyBehaviorRestrictionProperties> func, RequestOptions? options = null);

    /// <summary>
    ///     启用此行为限制。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。 </returns>
    Task EnableAsync(RequestOptions? options = null);

    /// <summary>
    ///     禁用此行为限制。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。 </returns>
    Task DisableAsync(RequestOptions? options = null);
}
