namespace Kook;

/// <summary>
///     表示一个通用的反应。
/// </summary>
public interface IReaction
{
    /// <summary>
    ///     获取此反应所使用的表情符号。
    /// </summary>
    IEmote Emote { get; }
}
