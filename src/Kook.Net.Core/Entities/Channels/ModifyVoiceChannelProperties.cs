namespace Kook;

/// <summary>
///     提供用于修改 <see cref="T:Kook.IVoiceChannel"/> 的属性。
/// </summary>
/// <seealso cref="M:Kook.IVoiceChannel.ModifyAsync(System.Action{Kook.ModifyVoiceChannelProperties},Kook.RequestOptions)"/>
public class ModifyVoiceChannelProperties : ModifyTextChannelProperties
{
    /// <summary>
    ///     获取或设置要设置到此频道的要求语音频道中的客户端使用的语音质量。
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         受限于 KOOK API，无法通过 KOOK API 设置语音频道的语音质量为 <see cref="F:Kook.VoiceQuality._128kbps"/>
    ///         或更高的值，尽管语音频道所属的服务器已有相应的服务器助力。通过此属性设置为不低于
    ///         <see cref="F:Kook.VoiceQuality._128kbps"/> 的语音质量的操作是无效的。
    ///     </note>
    ///     <br />
    ///     如果此值为 <c>null</c>，则语音质量不会被修改。
    /// </remarks>
    public VoiceQuality? VoiceQuality { get; set; }

    /// <summary>
    ///     获取或设置要设置到此频道的允许同时连接到此频道的最大用户数；如果没有限制，则为 <c>0</c>。
    /// </summary>
    /// <remarks>
    ///     如果此值为 <c>0</c>，则用户限制将被清除；如果此值为 <c>null</c>，则用户限制不会被修改。
    /// </remarks>
    public int? UserLimit { get; set; }

    /// <summary>
    ///     获取或设置要设置到此频道的密码。
    /// </summary>
    /// <remarks>
    ///     密码只支持 1 至 12 位的数字，设置密码后，用户连接到此频道时需要输入密码。如果此值为
    ///     <see cref="F:System.String.Empty"/>，则密码将被清除；如果此值为 <c>null</c>，则密码不会被修改。
    /// </remarks>
    public string? Password { get; set; }

    /// <summary>
    ///     获取或设置要设置到此频道的是否覆写了服务器默认设置的语音服务器区域。
    /// </summary>
    /// <remarks>
    ///     如果此值为 <c>null</c>，则不会修改是否覆写了服务器默认设置的语音服务器区域。
    /// </remarks>
    public bool? OverwriteVoiceRegion { get; set; }

    /// <summary>
    ///     获取或设置要设置到此频道的语音服务器区域。
    /// </summary>
    /// <remarks>
    ///     如果此值为 <c>null</c>，则语音服务器区域不会被修改。
    /// </remarks>
    /// <seealso cref="P:Kook.IGuild.Region"/>
    public string? VoiceRegion { get; set; }
}
