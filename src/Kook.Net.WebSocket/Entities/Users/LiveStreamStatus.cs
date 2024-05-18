using System.Diagnostics;
using Model = Kook.API.Gateway.LiveInfo;

namespace Kook.WebSocket;

/// <summary>
///     Represents the status of a live stream.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class LiveStreamStatus
{
    /// <summary>
    ///     Gets the voice channel that the user is currently in; or <c>null</c> if none.
    /// </summary>
    public SocketVoiceChannel? VoiceChannel { get; private set; }

    /// <summary>
    ///     Gets whether the stream is live.
    /// </summary>
    public bool IsLive { get; private set; }

    /// <summary>
    ///     Gets the number of audience members.
    /// </summary>
    public int AudienceCount { get; private set; }

    /// <summary>
    ///     Gets the maximum number of audience members.
    /// </summary>
    public int AudienceLimit { get; private set; }

    /// <summary>
    ///     Gets the thumbnail of the live stream.
    /// </summary>
    public string? CoverThumbnail { get; private set; }

    /// <summary>
    ///     Gets the start time of the live stream.
    /// </summary>
    public DateTimeOffset? StartTime { get; private set; }

    /// <summary>
    ///     Gets the resolution of the live stream.
    /// </summary>
    public int? Resolution { get; private set; }

    /// <summary>
    ///     Gets the frame rate of the live stream.
    /// </summary>
    public int? FrameRate { get; private set; }

    /// <summary>
    ///     Gets the tag of the live stream.
    /// </summary>
    public string Tag { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the color of the live stream.
    /// </summary>
    public AlphaColor Color { get; private set; }

    /// <summary>
    ///     Gets the image URL of the live stream.
    /// </summary>
    public string Image { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the mode of the live stream.
    /// </summary>
    public int Mode { get; private set; }

    private LiveStreamStatus()
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
