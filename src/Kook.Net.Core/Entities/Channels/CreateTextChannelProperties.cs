namespace Kook;

/// <summary>
///     提供用于创建 <see cref="Kook.ITextChannel"/> 的属性。
/// </summary>
/// <seealso cref="Kook.IGuild.CreateTextChannelAsync(System.String,System.Action{Kook.CreateTextChannelProperties},Kook.RequestOptions)"/>
public class CreateTextChannelProperties : CreateGuildChannelProperties
{
    /// <summary>
    ///     获取或设置要设置到此频道的所属分组频道的 ID。
    /// </summary>
    /// <remarks>
    ///     将此值设置为某分组频道的 ID 可以使新建频道位于该分组频道下；将此值设置为 <c>null</c>
    ///     可以使新建频道位于服务器所有分组频道的上方，即不属于任何分组频道。
    /// </remarks>
    public ulong? CategoryId { get; set; }
}
