namespace Kook;

/// <summary>
///     表示一个通用的回应。
/// </summary>
public interface IReaction
{
    /// <summary>
    ///     获取此回应所使用的表情符号。
    /// </summary>
    IEmote Emote { get; }
}
