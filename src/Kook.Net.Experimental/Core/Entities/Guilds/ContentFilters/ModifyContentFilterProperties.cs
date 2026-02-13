namespace Kook;

/// <summary>
///     提供用于修改 <see cref="IContentFilter"/> 的属性。
/// </summary>
public class ModifyContentFilterProperties
{
    /// <summary>
    ///     获取或设置要设置到此内容过滤器的目标。
    /// </summary>
    public IContentFilterTarget? Target { get; set; }

    /// <summary>
    ///     获取或设置要设置到此内容过滤器的豁免列表。
    /// </summary>
    public IReadOnlyCollection<ContentFilterExemption>? Exemptions { get; set; }

    /// <summary>
    ///     获取或设置要设置到此内容过滤器的处理程序。
    /// </summary>
    public IReadOnlyCollection<IContentFilterHandler>? Handlers { get; set; }

    /// <summary>
    ///     获取或设置要设置到此内容过滤器的启用状态。
    /// </summary>
    public bool? IsEnabled { get; set; }
}
