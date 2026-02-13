namespace Kook;

/// <summary>
///     表示一个内容过滤器的豁免实体。
/// </summary>
/// <param name="Type"> 获取此豁免的类型。 </param>
/// <param name="Id"> 获取此豁免的标识。 </param>
public readonly record struct ContentFilterExemption(ContentFilterExemptionType Type, ulong Id)
{
    /// <summary>
    ///     使用指定的服务器文字频道创建一个内容过滤器豁免实体。
    /// </summary>
    /// <param name="channel"> 要豁免的文字频道。 </param>
    public ContentFilterExemption(ITextChannel channel)
        : this(ContentFilterExemptionType.TextChannel, channel.Id)
    {
    }

    /// <summary>
    ///     使用指定的服务器分组频道创建一个内容过滤器豁免实体。
    /// </summary>
    /// <param name="channel"> 要豁免的分组频道。 </param>
    public ContentFilterExemption(ICategoryChannel channel)
        : this(ContentFilterExemptionType.CategoryChannel, channel.Id)
    {
    }

    /// <summary>
    ///     使用指定的服务器角色创建一个内容过滤器豁免实体。
    /// </summary>
    /// <param name="role"> 要豁免的服务器角色。 </param>
    public ContentFilterExemption(IRole role)
        : this(ContentFilterExemptionType.Role, role.Id)
    {
    }
}
