using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     提供用于各种帖子相关实体的扩展方法。
/// </summary>
public static class ThreadExtensions
{
    /// <summary>
    ///     获取一个跳转到帖子的 URL。
    /// </summary>
    /// <param name="thread"> 要获取跳转 URL 的帖子。 </param>
    /// <returns> 一个包含用于在聊天中跳转到帖子的 URL 的字符串。 </returns>
    public static string GetJumpUrl(this IThread thread) =>
        $"https://www.kookapp.cn/app/channels/{thread.Guild.Id}/{thread.Channel.Id}/{thread.Id}";

    /// <summary>
    ///     尝试将帖子内卡片的内容展开为单个字符串。
    /// </summary>
    /// <param name="thread"> 要展开的消息。 </param>
    /// <param name="extractedContent"> 展开的帖子。 </param>
    /// <returns> 如果成功展开，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool TryExtractCardContent(this IThread thread,
        [NotNullWhen(true)] out string? extractedContent)
    {
        string result = string.Join(" ", MessageExtensions.EnumerateCardModuleContents(thread.Cards));
        if (string.IsNullOrWhiteSpace(result))
        {
            extractedContent = null;
            return false;
        }

        extractedContent = result;
        return true;
    }

    /// <summary>
    ///     尝试将帖子评论内卡片的内容展开为单个字符串。
    /// </summary>
    /// <param name="post"> 要展开的帖子评论。 </param>
    /// <param name="extractedContent"> 展开的内容。 </param>
    /// <returns> 如果成功展开，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool TryExtractCardContent(this IThreadPost post,
        [NotNullWhen(true)] out string? extractedContent)
    {
        string result = string.Join(" ", MessageExtensions.EnumerateCardModuleContents(post.Cards));
        if (string.IsNullOrWhiteSpace(result))
        {
            extractedContent = null;
            return false;
        }

        extractedContent = result;
        return true;
    }

    /// <summary>
    ///     尝试将帖子评论内卡片的内容展开为单个字符串。
    /// </summary>
    /// <param name="reply"> 要展开的帖子评论。 </param>
    /// <param name="extractedContent"> 展开的内容。 </param>
    /// <returns> 如果成功展开，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool TryExtractCardContent(this IThreadReply reply,
        [NotNullWhen(true)] out string? extractedContent)
    {
        string result = string.Join(" ", MessageExtensions.EnumerateCardModuleContents(reply.Cards));
        if (string.IsNullOrWhiteSpace(result))
        {
            extractedContent = null;
            return false;
        }

        extractedContent = result;
        return true;
    }
}
