using System.Text;
using System.Text.RegularExpressions;

namespace Kook;

/// <summary>
///     A helper class for formatting characters.
/// </summary>
public static class Format
{
    private static readonly string[] SensitiveCharacters = { "\\", "*", "~", "`", ":", "-", "]", ")", ">" };

    /// <summary> Returns a markdown-formatted string with bold formatting. </summary>
    public static string Bold(string text) => $"**{text}**";

    /// <summary> Returns a markdown-formatted string with italics formatting. </summary>
    public static string Italics(string text) => $"*{text}*";

    /// <summary> Returns a markdown-formatted string with strike-through formatting. </summary>
    public static string Strikethrough(string text) => $"~~{text}~~";

    /// <summary> Returns a markdown-formatted string colored with the specified <see cref="TextTheme"/>. </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         Colored text is only supported in cards.
    ///     </note>
    /// </remarks>
    public static string Colorize(string text, TextTheme theme) =>
        $"(font){text}(font)[{theme.ToString().ToLowerInvariant()}]";

    /// <summary> Returns a markdown-formatted URL. </summary>
    public static string Url(string text, string url) => $"[{text}]({url})";

    /// <summary> Sanitizes the string, safely escaping any Markdown sequences. </summary>
    public static string Sanitize(string text)
    {
        if (text is null) return null;

        return SensitiveCharacters.Aggregate(text,
            (current, unsafeChar) => current.Replace(unsafeChar, $"\\{unsafeChar}"));
    }

    /// <summary>
    ///     Formats a string as a quote.
    /// </summary>
    /// <param name="text">The text to format.</param>
    /// <returns>Gets the formatted quote text.</returns>
    public static string Quote(string text)
    {
        // do not modify null or whitespace text
        // whitespace does not get quoted properly
        if (string.IsNullOrWhiteSpace(text)) return text;

        StringBuilder result = new();

        int startIndex = 0;
        int newLineIndex;
        do
        {
            newLineIndex = text.IndexOf('\n', startIndex);
            if (newLineIndex == -1)
            {
                // read the rest of the string
                string str = text.Substring(startIndex);
                result.Append($"> {str}");
            }
            else
            {
                // read until the next newline
                string str = text.Substring(startIndex, newLineIndex - startIndex);
                result.Append($"> {str}\n");
            }

            startIndex = newLineIndex + 1;
        } while (newLineIndex != -1 && startIndex != text.Length);

        return result.ToString();
    }

    /// <summary> Returns a markdown-formatted string with underline formatting. </summary>
    public static string Underline(string text) => $"(ins){text}(ins)";

    /// <summary> Returns a string with spoiler formatting. </summary>
    public static string Spoiler(string text) => $"(spl){text}(spl)";

    /// <summary> Returns a markdown-formatted string with code block formatting. </summary>
    public static string Code(string text, string language = null)
    {
        if (language != null || text.Contains('\n'))
            return $"```{language ?? ""}\n{text}\n```";
        else
            return $"`{text}`";
    }

    /// <summary>
    ///     Formats a string as a block quote.
    /// </summary>
    /// <param name="text">The text to format.</param>
    /// <returns>Gets the formatted block quote text.</returns>
    public static string BlockQuote(string text)
    {
        // do not modify null or whitespace
        if (string.IsNullOrWhiteSpace(text)) return text;

        return $">>> {text}";
    }

    /// <summary>
    /// Remove Kook supported markdown from text.
    /// </summary>
    /// <param name="text">The to remove markdown from.</param>
    /// <returns>Gets the unformatted text.</returns>
    public static string StripMarkDown(string text)
    {
        // // Remove color
        // var newText = Regex.Replace(text,
        //     @"(?<!\\)(?:\\.)*\(font\)(?<text>.+?)(?<!\\)(?:\\.)*\(font\)\[\w+\]",
        //     @"${text}",
        //     RegexOptions.Compiled | RegexOptions.RightToLeft);

        // Remove Kook supported markdown
        string newText = Regex.Replace(text, @"(\*|\(ins\)|\(spl\)|`|~|>|\\)", "");

        return newText;
    }

    /// <summary>
    ///     Formats a user's username + identify number while maintaining bidirectional unicode
    /// </summary>
    /// <param name="user">The user whose username and identify number to format</param>
    /// <param name="doBidirectional">To format the string in bidirectional unicode or not</param>
    /// <returns>The username + identify number</returns>
    public static string UsernameAndIdentifyNumber(IUser user, bool doBidirectional) =>
        doBidirectional
            ? $"\u2066{user.Username}\u2069#{user.IdentifyNumber}"
            : $"{user.Username}#{user.IdentifyNumber}";
}
