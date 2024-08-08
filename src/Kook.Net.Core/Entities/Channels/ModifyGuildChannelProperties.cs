namespace Kook;

/// <summary>
///     提供用于修改 <see cref="T:Kook.IGuildChannel" /> 的属性。
/// </summary>
/// <seealso cref="M:Kook.IGuildChannel.ModifyAsync(System.Action{Kook.ModifyGuildChannelProperties},Kook.RequestOptions)"/>
public class ModifyGuildChannelProperties
{
    /// <summary>
    ///     获取或设置要设置到此频道的新名称。
    /// </summary>
    /// <remarks>
    ///     如果此值为 <c>null</c>，则频道的名称不会被修改。
    /// </remarks>
    public string? Name { get; set; }

    /// <summary>
    ///     获取或设置要设置到此频道的新位置。
    /// </summary>
    /// <remarks>
    ///     更小的数值表示更靠近列表顶部的位置。设置为与同分组下的其他频道相同的值，将会使当前频道排列于与该频道相邻更靠近列表顶部的位置。
    ///     如果此值为 <c>null</c>，则频道的位置不会被修改。
    /// </remarks>
    public int? Position { get; set; }

    /// <summary>
    ///     获取或设置要设置到此频道的所属分组频道的 ID。
    /// </summary>
    /// <remarks>
    ///     设置此值为某分组频道的 ID 将会使当前频道移动至该分组频道下；设置此值为 <c>0</c> 将会使当前频道脱离其当前所属的分组频道，
    ///     位于所有分组频道的上方；如果此值为 <c>null</c>，则当前频道的所属分组频道不会被修改。
    /// </remarks>
    public ulong? CategoryId { get; set; }
}
