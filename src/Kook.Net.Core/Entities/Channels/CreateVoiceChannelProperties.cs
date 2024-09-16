namespace Kook;

/// <summary>
///     提供用于创建 <see cref="Kook.IVoiceChannel"/> 的属性。
/// </summary>
/// <seealso cref="Kook.IGuild.CreateVoiceChannelAsync(System.String,System.Action{Kook.CreateVoiceChannelProperties},Kook.RequestOptions)"/>
public class CreateVoiceChannelProperties : CreateTextChannelProperties
{
    /// <summary>
    ///     获取或设置要求语音频道中的客户端使用的语音质量。
    /// </summary>
    public VoiceQuality? VoiceQuality { get; set; }

    /// <summary>
    ///     获取或设置允许同时连接到此频道的最大用户数；<c>null</c> 表示不限制。
    /// </summary>
    public int? UserLimit { get; set; }
}
