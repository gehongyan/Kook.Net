using System.Diagnostics.CodeAnalysis;

namespace Kook.Commands;

/// <summary>
///     Provides extension methods for <see cref="IUserMessage" /> that relates to commands.
/// </summary>
public static class MessageExtensions
{
    /// <summary>
    ///     Gets whether the message starts with the provided character.
    /// </summary>
    /// <param name="msg">The message to check against.</param>
    /// <param name="c">The char prefix.</param>
    /// <param name="argPos">References where the command starts.</param>
    /// <returns>
    ///     <c>true</c> if the message begins with the char <paramref name="c"/>; otherwise <c>false</c>.
    /// </returns>
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
    ///     Gets whether the message starts with the provided string.
    /// </summary>
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
    ///     Gets whether the message starts with the user's mention string.
    /// </summary>
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

    /// <summary>
    ///     Tries to expand the content of the card into a single string.
    /// </summary>
    /// <param name="msg"> The message to expand the content of. </param>
    /// <param name="expandedContent"> The expanded content of the card. </param>
    /// <returns> <c>true</c> if the content was successfully expanded; otherwise, <c>false</c>. </returns>
    public static bool TryExpandCardContent(this IUserMessage msg,
        [NotNullWhen(true)] out string? expandedContent)
    {
        if (!msg.MaybeTextImageMixedMessage())
        {
            expandedContent = null;
            return false;
        }

        string result = string.Join(" ", EnumerateCardModuleContents(msg.Cards));
        if (string.IsNullOrWhiteSpace(result))
        {
            expandedContent = null;
            return false;
        }

        expandedContent = result;
        return true;
    }

    private static IEnumerable<string> EnumerateCardModuleContents(IEnumerable<ICard> cards) => cards
        .OfType<Card>()
        .SelectMany(x => x.Modules)
        .Select(x => x switch
        {
            SectionModule { Text: PlainTextElement or KMarkdownElement } sectionModule =>
                sectionModule.Text.ToString(),
            ContainerModule { Elements: [{ } element] } => element.Source,
            _ => null
        })
        .OfType<string>()
        .Where(x => !string.IsNullOrWhiteSpace(x));
}
