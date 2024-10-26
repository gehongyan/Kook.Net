namespace Kook;

/// <summary>
///     提供用于修改 <see cref="Kook.IThreadChannel"/> 的属性。
/// </summary>
/// <seealso cref="Kook.IThreadChannel.ModifyAsync(System.Action{Kook.ModifyThreadChannelProperties},Kook.RequestOptions)"/>
public class ModifyThreadChannelProperties : ModifyGuildChannelProperties
{
    /// <summary>
    ///     获取或设置要设置到此频道的说明。
    /// </summary>
    /// <remarks>
    ///     如果此值为 <c>null</c>，则频道的说明不会被修改；如果此值为 <see cref="System.String.Empty"/>，则频道的说明将被清空；
    ///     设置为其他值将会修改频道的说明。
    /// </remarks>
    public string? Topic { get; set; }

    /// <summary>
    ///     获取或设置要设置到此频道的发帖慢速模式延迟。
    /// </summary>
    /// <remarks>
    ///     设置此值将要求每个用户在发布另一条发帖之前等待指定的时间间隔；设置为 <see cref="Kook.SlowModeInterval.None"/>
    ///     将会为此频道禁用发帖慢速模式；如果此值为 <c>null</c>，则发帖慢速模式延迟不会被修改。
    ///     <note>
    ///         拥有 <see cref="Kook.ChannelPermission.ManageMessages"/> 或
    ///         <see cref="Kook.ChannelPermission.ManageChannels"/> 权限的用户不受慢速模式延迟的限制。
    ///     </note>
    /// </remarks>
    public SlowModeInterval? PostCreationInterval { get; set; }

    /// <summary>
    ///     获取或设置要设置到此频道的回帖慢速模式延迟。
    /// </summary>
    /// <remarks>
    ///     设置此值将要求每个用户在对任意帖子发布另一条回复之前等待指定的时间间隔；设置为 <see cref="Kook.SlowModeInterval.None"/>
    ///     将会为此频道禁用会回帖慢速模式；如果此值为 <c>null</c>，则发帖慢速模式延迟不会被修改。
    ///     <note>
    ///         拥有 <see cref="Kook.ChannelPermission.ManageMessages"/> 或
    ///         <see cref="Kook.ChannelPermission.ManageChannels"/> 权限的用户不受回帖慢速模式延迟的限制。
    ///     </note>
    /// </remarks>
    public SlowModeInterval? ReplyInterval { get; set; }
}
