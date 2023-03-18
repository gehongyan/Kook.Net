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
        string text = msg.Content;
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
    public static bool HasStringPrefix(this IUserMessage msg, string str, ref int argPos, StringComparison comparisonType = StringComparison.Ordinal)
    {
        string text = msg.Content;
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
        if (msg.Type == MessageType.Text)
        {
            string text = msg.Content;
            if (string.IsNullOrEmpty(text) || text.Length <= 6 || text[0] != '@') return false;

            int endPos = text.IndexOf('#');
            if (endPos == -1) return false;

            endPos += 4;
            if (text.Length < endPos + 2 || text[endPos + 1] != ' ') return false;

            if (!MentionUtils.TryParseUser(text.Substring(0, endPos + 1), out ulong userId, TagMode.PlainText)) return false;

            if (userId == user.Id)
            {
                argPos = endPos + 2;
                return true;
            }
        }
        else if (msg.Type == MessageType.KMarkdown)
        {
            string text = msg.Content;
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            if (string.IsNullOrEmpty(text) || text.Length <= 10 || text[..5] != "(met)") return false;
#else
                if (string.IsNullOrEmpty(text) || text.Length <= 10 || text.Substring(0, 5) != "(met)") return false;
#endif

            int endPos = text.IndexOf("(met)", 5, StringComparison.Ordinal);
            if (endPos == -1) return false;

            endPos += 4;
            if (text.Length < endPos + 2 || text[endPos + 1] != ' ') return false;

            if (!MentionUtils.TryParseUser(text.Substring(0, endPos + 1), out ulong userId, TagMode.KMarkdown)) return false;

            if (userId == user.Id)
            {
                argPos = endPos + 2;
                return true;
            }
        }

        return false;
    }
}
