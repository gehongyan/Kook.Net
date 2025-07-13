using System.Diagnostics.CodeAnalysis;

namespace Kook.Commands;

/// <summary>
///     提供用于 <see cref="Kook.IUserMessage" /> 与命令相关的扩展方法。
/// </summary>
public static class MessageExtensions
{
    /// <summary>
    ///     获取消息是否以提供的字符开头。
    /// </summary>
    /// <param name="msg"> 要检查的消息。 </param>
    /// <param name="c"> 要检查的前导字符。 </param>
    /// <param name="argPos"> 开始检查的位置。 </param>
    /// <returns> 如果消息以指定的字符开头，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool HasCharPrefix(this IUserMessage msg, char c, ref int argPos)
    {
        string text;
        if (msg.MaybeTextImageMixedMessage())
        {
            IModule? module = msg.Cards
                .OfType<Card>()
                .SelectMany(x => x.Modules)
                .FirstOrDefault();
            if (module is not SectionModule sectionModule) return false;
            switch (sectionModule.Text)
            {
                case KMarkdownElement kMarkdownElement:
                    text = kMarkdownElement.Content;
                    break;
                case PlainTextElement plainTextElement:
                    text = plainTextElement.Content;
                    break;
                default:
                    return false;
            }
        }
        else
            text = msg.Content;

        if (!string.IsNullOrEmpty(text) && text[0] == c)
        {
            argPos = 1;
            return true;
        }

        return false;
    }

    /// <summary>
    ///     获取消息是否以提供的字符串开头。
    /// </summary>
    /// <param name="msg"> 要检查的消息。 </param>
    /// <param name="str"> 要检查的前导字符。 </param>
    /// <param name="argPos"> 开始检查的位置。 </param>
    /// <param name="comparisonType"> 字符串比较模式。 </param>
    /// <returns> 如果消息以指定的字符串开头，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool HasStringPrefix(this IUserMessage msg, string str,
        ref int argPos, StringComparison comparisonType = StringComparison.Ordinal)
    {
        string text;
        if (msg.MaybeTextImageMixedMessage())
        {
            IModule? module = msg.Cards
                .OfType<Card>()
                .SelectMany(x => x.Modules)
                .FirstOrDefault();
            if (module is not SectionModule sectionModule) return false;
            switch (sectionModule.Text)
            {
                case KMarkdownElement kMarkdownElement:
                    text = kMarkdownElement.Content;
                    break;
                case PlainTextElement plainTextElement:
                    text = plainTextElement.Content;
                    break;
                default:
                    return false;
            }
        }
        else
            text = msg.Content;

        if (!string.IsNullOrEmpty(text) && text.StartsWith(str, comparisonType))
        {
            argPos = str.Length;
            return true;
        }

        return false;
    }

    /// <summary>
    ///     获取消息是否以提供的用户提及开头。
    /// </summary>
    /// <param name="msg"> 要检查的消息。 </param>
    /// <param name="user"> 要检查的用户。 </param>
    /// <param name="argPos"> 开始检查的位置。 </param>
    /// <returns> 如果消息以指定的用户提及开头，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool HasMentionPrefix(this IUserMessage msg, IUser user, ref int argPos)
    {
        string text;
        MessageType type;
        if (msg.MaybeTextImageMixedMessage())
        {
            IModule? module = msg.Cards
                .OfType<Card>()
                .SelectMany(x => x.Modules)
                .FirstOrDefault();
            if (module is not SectionModule sectionModule) return false;
            switch (sectionModule.Text)
            {
                case KMarkdownElement kMarkdownElement:
                    text = kMarkdownElement.Content;
                    type = MessageType.KMarkdown;
                    break;
                case PlainTextElement plainTextElement:
                    text = plainTextElement.Content;
                    type = MessageType.Text;
                    break;
                default:
                    return false;
            }
        }
        else if (msg.Type is MessageType.Text or MessageType.KMarkdown)
        {
            text = msg.Content;
            type = msg.Type;
        }
        else
            return false;

        if (type == MessageType.Text)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= 6 || text[0] != '@')
                return false;

            int endPos = text.IndexOf('#');
            if (endPos == -1)
                return false;

            endPos += 4;
            if (text.Length < endPos + 2 || text[endPos + 1] != ' ')
                return false;

            if (!MentionUtils.TryParseUser(text.Substring(0, endPos + 1), out ulong userId, TagMode.PlainText))
                return false;

            if (userId == user.Id)
            {
                argPos = endPos + 2;
                return true;
            }

            return false;
        }

        if (type == MessageType.KMarkdown)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= 10 || text[..5] != "(met)")
                return false;

            int endPos = text.IndexOf("(met)", 5, StringComparison.Ordinal);
            if (endPos == -1)
                return false;

            endPos += 4;
            if (text.Length < endPos + 2 || text[endPos + 1] != ' ')
                return false;

            if (!MentionUtils.TryParseUser(text.Substring(0, endPos + 1), out ulong userId, TagMode.KMarkdown))
                return false;

            if (userId == user.Id)
            {
                argPos = endPos + 2;
                return true;
            }

            return false;
        }

        return false;
    }

    /// <inheritdoc cref="Kook.MessageExtensions.TryExtractCardContent" />
    [Obsolete("Use `Kook.MessageExtensions.TryExtractCardContent(IUserMessage, out string)` instead.")]
    public static bool TryExpandCardContent(this IUserMessage msg,
        [NotNullWhen(true)] out string? expandedContent) =>
        msg.TryExtractCardContent(out expandedContent);
}
