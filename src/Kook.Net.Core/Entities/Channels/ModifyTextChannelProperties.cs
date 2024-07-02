namespace Kook;

/// <summary>
///     提供用于修改 <see cref="T:Kook.ITextChannel"/> 的属性。
/// </summary>
/// <seealso cref="M:Kook.ITextChannel.ModifyAsync(System.Action{Kook.ModifyTextChannelProperties},Kook.RequestOptions)"/>
public class ModifyTextChannelProperties : ModifyGuildChannelProperties
{
    /// <summary>
    ///     获取或设置要设置到此频道的说明。
    /// </summary>
    /// <remarks>
    ///     如果此值为 <c>null</c>，则频道的说明不会被修改；如果此值为 <see cref="F:System.String.Empty"/>，则频道的说明将被清空；
    ///     设置为其他值将会修改频道的说明。
    /// </remarks>
    public string? Topic { get; set; }

    /// <summary>
    ///     获取或设置此要设置到此频道的慢速模式延迟。
    /// </summary>
    /// <remarks>
    ///     设置此值将要求每个用户在发送另一条消息之前等待指定的时间间隔；设置为 <see cref="F:Kook.SlowModeInterval.None"/>
    ///     将会为此频道禁用慢速模式；如果此值为 <c>null</c>，则慢速模式延迟不会被修改。
    ///     <note>
    ///         拥有 <see cref="F:Kook.ChannelPermission.ManageMessages"/> 或
    ///         <see cref="F:Kook.ChannelPermission.ManageChannels"/> 权限的用户不受慢速模式延迟的限制。
    ///     </note>
    /// </remarks>
    public SlowModeInterval? SlowModeInterval { get; set; }
}
