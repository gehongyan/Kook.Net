namespace KaiHeiLa;

/// <summary>
///     语音频道
/// </summary>
public interface IVoiceChannel : INestedChannel, IAudioChannel, IMentionable
{
    VoiceQuality VoiceQuality { get; }
    
    int? UserLimit { get; }

    string ServerUrl { get; }
}