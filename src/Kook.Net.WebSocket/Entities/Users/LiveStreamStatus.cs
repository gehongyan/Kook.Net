using System.Diagnostics;
using Model = Kook.API.Gateway.LiveInfo;

namespace Kook.WebSocket;

/// <summary>
///     表示一个直播状态。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct LiveStreamStatus
{
    /// <summary>
    ///     获取用户当前所在的语音频道；如果不在任何频道中则为 <c>null</c>。
    /// </summary>
    public SocketVoiceChannel? VoiceChannel { get; private set; }

    /// <summary>
    ///     获取此直播是否正在进行。
    /// </summary>
    public bool IsLive { get; private set; }

    /// <summary>
    ///     获取此直播的观众人数。
    /// </summary>
    public int AudienceCount { get; private set; }

    /// <summary>
    ///     获取此直播间可容纳的最大观众人数。
    /// </summary>
    public int AudienceLimit { get; private set; }

    /// <summary>
    ///     获取此直播的封面缩略图。
    /// </summary>
    public string? CoverThumbnail { get; private set; }

    /// <summary>
    ///     获取此直播的开始时间。
    /// </summary>
    public DateTimeOffset? StartTime { get; private set; }

    /// <summary>
    ///     获取此直播的分辨率。
    /// </summary>
    public int? Resolution { get; private set; }

    /// <summary>
    ///     获取此直播的帧率。
    /// </summary>
    public int? FrameRate { get; private set; }

    /// <summary>
    ///     获取此直播的标签。
    /// </summary>
    public string Tag { get; private set; } = string.Empty;

    /// <summary>
    ///     // TODO: To be documented.
    /// </summary>
    public AlphaColor Color { get; private set; }

    /// <summary>
    ///     获取此直播间图像的 URL。
    /// </summary>
    public string Image { get; private set; } = string.Empty;

    /// <summary>
    ///     获取直播间的模式。
    /// </summary>
    public int Mode { get; private set; }

    /// <summary>
    ///     初始化 <see cref="LiveStreamStatus"/> 结构的新实例。
    /// </summary>
    public LiveStreamStatus()
    {
    }

    internal static LiveStreamStatus Create(SocketVoiceChannel? voiceChannel, Model model)
    {
        LiveStreamStatus status = new();
        status.Update(voiceChannel, model);
        return status;
    }

    internal void Update(SocketVoiceChannel? voiceChannel, Model model)
    {
        VoiceChannel = voiceChannel;
        IsLive = model.InLive;
        AudienceCount = model.AudienceCount;
        AudienceLimit = model.AudienceLimit;
        CoverThumbnail = model.LiveThumb;
        StartTime = model.LiveStartTime;
        Resolution = model.Resolution;
        FrameRate = model.FrameRate;
        Tag = model.Tag;
        Color = model.Color;
        Image = model.ImgUrl;
        Mode = model.Mode;
    }

    private string DebuggerDisplay => IsLive ? "Live" : "Not Live";
}
