namespace Kook;

/// <summary>
///     提供对表情符号的扩展方法。
/// </summary>
public static class EmoteExtensions
{
    /// <summary>
    ///     获取互动表情随机结果的资源 URL。
    /// </summary>
    /// <param name="emoteResult"> 要获取资源 URL 的互动表情随机结果。</param>
    /// <returns> 互动表情随机结果的资源 URL。</returns>
    public static string GetResourceUrl(this InteractiveEmoteRollResult emoteResult) =>
        $"{KookConfig.InteractiveEmoteResourceUrl}{emoteResult.Image}";

    /// <summary>
    ///     获取互动表情符号在显示随机结果前的动画资源图像的 URL。
    /// </summary>
    /// <param name="emote"> 要获取动画资源 URL 的互动表情。</param>
    /// <returns> 互动表情随机结果的资源 URL。</returns>
    /// <remarks>
    ///     如果动态表情符号不是从 KOOK API 或网关生成的，则此方法返回 <c>null</c>。
    /// </remarks>
    public static string? GetDynamicResourceUrl(this InteractiveEmote emote) =>
        !string.IsNullOrWhiteSpace(emote.DynamicImage)
            ? $"{KookConfig.InteractiveEmoteResourceUrl}{emote.DynamicImage}"
            : null;
}
