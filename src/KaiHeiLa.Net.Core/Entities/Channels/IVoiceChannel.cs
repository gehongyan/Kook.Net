namespace KaiHeiLa;

/// <summary>
///     语音频道
/// </summary>
public interface IVoiceChannel : INestedChannel, IAudioChannel, IMentionable
{
    VoiceQuality VoiceQuality { get; }
    
    int LimitAmount { get; }

    string ServerUrl { get; }
}