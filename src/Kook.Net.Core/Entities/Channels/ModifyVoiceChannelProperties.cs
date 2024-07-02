namespace Kook;

/// <summary>
///     Provides properties that are used to modify an <see cref="IVoiceChannel"/> with the specified properties.
/// </summary>
/// <seealso cref="IVoiceChannel.ModifyAsync(System.Action{ModifyVoiceChannelProperties}, RequestOptions)"/>
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
    ///     语音服务器区域是指语音服务器所在的地理位置，各个语音服务器区域由一个唯一的字符串表示。 <br />
    ///     可用语音服务器区域参考列表：
    ///     <list type="table">
    ///         <listheader>
    ///             <term> 区域 ID </term>
    ///             <description> 区域名称 </description>
    ///         </listheader>
    ///         <item>
    ///             <term> <c>chengdu</c> </term>
    ///             <description> 西南(成都) </description>
    ///         </item>
    ///         <item>
    ///             <term> <c>beijing</c> </term>
    ///             <description> 华北(北京) </description>
    ///         </item>
    ///         <item>
    ///             <term> <c>shanghai</c> </term>
    ///             <description> 华东(上海) </description>
    ///         </item>
    ///         <item>
    ///             <term> <c>shenzhen</c> </term>
    ///             <description> 华南(深圳) </description>
    ///         </item>
    ///         <item>
    ///             <term> <c>hk</c> </term>
    ///             <description> 亚太(香港) </description>
    ///         </item>
    ///         <item>
    ///             <term> <c>vnga</c> </term>
    ///             <description> 国际专线(助力专享) </description>
    ///         </item>
    ///     </list>
    ///     此列表仅供参考。要获取最新可用服务器区域列表，可在 Kook.Net.Experimental 实验性 API 实现包中，在
    ///     <see cref="T:Kook.Rest.BaseKookClient"/> 上调用 <c>GetVoiceRegionsAsync</c> 方法。 <br />
    ///     如果此值为 <c>null</c>，则语音服务器区域不会被修改。
    /// </remarks>
    public string? VoiceRegion { get; set; }
}
