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
    /// <param name="text">The text to format.</param>
    /// <param name="sanitize"> Whether to sanitize the text. </param>
    /// <returns>Gets the formatted text.</returns>
    /// <remarks>
    ///     Set <paramref name="sanitize"/> to <c>true</c> will sanitize the text by replacing all occurrences of
    ///     <c>*</c> with <c>\*</c>.
    /// </remarks>
    public static string Bold(this string text, bool sanitize = true) =>
        $"**{(sanitize ? text.Sanitize("*") : text)}**";

    /// <summary> Returns a markdown-formatted string with italics formatting. </summary>
    /// <param name="text">The text to format.</param>
    /// <param name="sanitize"> Whether to sanitize the text. </param>
    /// <returns>Gets the formatted text.</returns>
    /// <remarks>
    ///     Set <paramref name="sanitize"/> to <c>true</c> will sanitize the text by replacing all occurrences of
    ///     <c>*</c> with <c>\*</c>.
    /// </remarks>
    public static string Italics(this string text, bool sanitize = true) =>
        $"*{(sanitize ? text.Sanitize("*") : text)}*";

    /// <summary> Returns a markdown-formatted string with bold italics formatting. </summary>
    /// <param name="text">The text to format.</param>
    /// <param name="sanitize"> Whether to sanitize the text. </param>
    /// <returns>Gets the formatted text.</returns>
    /// <remarks>
    ///     Set <paramref name="sanitize"/> to <c>true</c> will sanitize the text by replacing all occurrences of
    ///     <c>*</c> with <c>\*</c>.
    /// </remarks>
    public static string BoldItalics(this string text, bool sanitize = true) =>
        $"***{(sanitize ? text.Sanitize("*") : text)}***";

    /// <summary> Returns a markdown-formatted string with strike-through formatting. </summary>
    /// <param name="text">The text to format.</param>
    /// <param name="sanitize"> Whether to sanitize the text. </param>
    /// <returns>Gets the formatted text.</returns>
    /// <remarks>
    ///     Set <paramref name="sanitize"/> to <c>true</c> will sanitize the text by replacing all occurrences of
    ///     <c>~</c> with <c>\~</c>.
    /// </remarks>
    public static string Strikethrough(this string text, bool sanitize = true) =>
        $"~~{(sanitize ? text.Sanitize("~") : text)}~~";

    /// <summary> Returns a markdown-formatted string colored with the specified <see cref="TextTheme"/>. </summary>
    /// <param name="text">The text to colorize.</param>
    /// <param name="theme"> The theme to colorize the text with. </param>
    /// <param name="sanitize"> Whether to sanitize the text. </param>
    /// <returns>Gets the colorized text.</returns>
    /// <remarks>
    ///     <note type="warning">
    ///         Colored text is only supported in cards.
    ///     </note>
    /// </remarks>
    /// <remarks>
    ///     Set <paramref name="sanitize"/> to <c>true</c> will sanitize the text by replacing all occurrences of
    ///     <c>(</c> and <c>)</c> with <c>\(</c> and <c>\)</c>.
    /// </remarks>
    public static string Colorize(this string text, TextTheme theme, bool sanitize = true) =>
        $"(font){(sanitize ? text.Sanitize("(", ")") : text)}(font)[{theme.ToString().ToLowerInvariant()}]";

    /// <summary> Returns a markdown-formatted URL. </summary>
    /// <param name="text">The text to format.</param>
    /// <param name="url"> The URL to format. </param>
    /// <param name="sanitize"> Whether to sanitize the text and URL. </param>
    /// <returns>Gets the formatted URL.</returns>
    /// <remarks>
    ///     Set <paramref name="sanitize"/> to <c>true</c> will sanitize the text by replacing all occurrences of
    ///     <c>[</c> and <c>]</c> with <c>\[</c> and <c>\]</c>, and the URL by replacing all occurrences of
    ///     <c>(</c> and <c>)</c> with <c>\(</c> and <c>\)</c>.
    /// </remarks>
    public static string Url(this string text, string url, bool sanitize = true) =>
        $"[{(sanitize ? text.Sanitize("[", "]") : text)}]({(sanitize ? url.Sanitize("(", ")") : url)})";


    /// <inheritdoc cref="Url(string,string,bool)" />
    public static string Url(this string text, Uri url, bool sanitize = true) => text.Url(url.ToString(), sanitize);

    /// <summary> Sanitizes the string, safely escaping any Markdown sequences. </summary>
    /// <param name="text">The text to sanitize.</param>
    /// <param name="sensitiveCharacters"> The characters to sanitize. </param>
    /// <returns>Gets the sanitized text.</returns>
    /// <remarks>
    ///     If no sensitive characters are specified, the default sensitive characters are used.
    ///     The default sensitive characters are: <c>\</c>, <c>*</c>, <c>~</c>, <c>`</c>, <c>:</c>, <c>-</c>, <c>]</c>, <c>)</c>, <c>&gt;</c>.
    /// </remarks>
    public static string Sanitize(this string text, params string[] sensitiveCharacters)
    {
        if (text is null) return null;

        if (sensitiveCharacters is not { Length: > 0 })
            return SensitiveCharacters.Aggregate(text,
                (current, unsafeChar) => current.Replace(unsafeChar, $"\\{unsafeChar}"));
        return sensitiveCharacters.Aggregate(text,
            (current, unsafeChar) => current.Replace(unsafeChar, $"\\{unsafeChar}"));
    }

    /// <summary>
    ///     Formats a string as split quotes seperated by multiple new lines.
    /// </summary>
    /// <param name="text">The text to format.</param>
    /// <param name="sanitize"> Whether to sanitize the text. </param>
    /// <returns>Gets the formatted quote text.</returns>
    /// <remarks>
    ///     <note type="warning">
    ///         Due to the mechanism of the KOOK KMarkdown renderer, this method recognizes multiple text blocks
    ///         based on two or more consecutive line breaks, and formats each block as a quote. For each text block,
    ///         a greater than sign (<c>&gt;</c>) and a space is inserted at the beginning of the string, and a
    ///         zero-width joiner (<c>\u200d</c>) is inserted when the beginning of the block is a whitespace
    ///         character, to ensure proper display of the quote. When the user copies the text inside
    ///         the quote, they will not copy this special character. However, if you obtain the
    ///         message text from the server through message-related APIs, it will contain the
    ///         special character inserted by this method. An additional line break is also appended
    ///         at the end of the block to correct any missing line breaks caused by the quote formatting.
    ///         The appended line break is consistent with the style of the nearby line breaks.
    ///         To quote the entire text as a whole, use <see cref="BlockQuote"/> instead.
    ///     </note>
    ///     <para>
    ///         Set <paramref name="sanitize"/> to <c>true</c> will sanitize the text by replacing all occurrences of
    ///         <c>&gt;</c> with <c>\&gt;</c>.
    ///     </para>
    /// </remarks>
    /// <seealso cref="BlockQuote"/>
    public static string Quote(this string text, bool sanitize = true)
    {
        string escaped = sanitize ? text.Sanitize(">") : text;

        // do not modify null or whitespace text
        // whitespace does not get quoted properly
        if (string.IsNullOrWhiteSpace(escaped)) return escaped;

        StringBuilder result = new();

        string lineEnding = null;
        int textLength = escaped.Length;
        for (int i = 0; i < textLength; i++)
        {
            // Checks for the seperated block beginning
            if (escaped[i] is not ('\r' or '\n')
                && (i == 0
                    // The text begins with a new line then current character is not a new line
                    // CR or LF style
                    || (i == 1 && escaped[0] is '\r' or '\n')
                    // CRLF style
                    || (i == 2 && escaped[0] == '\r' && escaped[1] == '\n')
                    // Two new lines before current character
                    || (i > 1
                        // CR or LF style
                        && ((escaped[i - 2] == '\n' && escaped[i - 1] is '\r' or '\n')
                            || (escaped[i - 2] == '\r' && escaped[i - 1] == '\r')
                            // CRLF style
                            || (i > 2
                                && escaped[i - 3] is '\r' or '\n'
                                && escaped[i - 2] == '\r'
                                && escaped[i - 1] == '\n')))))
            {
                // The first character of the text
                result.Append("> ");
                if (char.IsWhiteSpace(escaped[i]))
                    result.Append('\u200d');
            }

            // Adds current character
            result.Append(escaped[i]);

            // Checks for nearby line ending
            if (escaped[i] == '\n' && i > 0 && escaped[i - 1] == '\r')
                lineEnding = "\r\n";
            else if (escaped[i] is '\r' or '\n')
                lineEnding = escaped[i].ToString();

            // Checks for the end of the seperated block to fix missing new line after quoting
            if (escaped[i] is not ('\r' or '\n'))
            {
                // The last character of the text
                if (i == textLength - 1)
                    result.Append(lineEnding ?? "\n");

                // Followed by a new line then the end of the text
                // CR or LF style
                else if (i == textLength - 2 && escaped[i + 1] is '\r' or '\n')
                {
                    result.Append(escaped[i + 1]);
                    lineEnding = escaped[i + 1].ToString();
                }

                // CRLF style
                else if (i == textLength - 3 && escaped[i + 1] == '\r' && escaped[i + 2] == '\n')
                {
                    result.Append("\r\n");
                    lineEnding = "\r\n";
                }

                // Followed by two new lines
                else if (i < textLength - 2
                         // CR or LF style
                         && ((escaped[i + 1] == '\n' && escaped[i + 2] is '\r' or '\n')
                             || (escaped[i + 1] == '\r' && escaped[i + 2] == '\r')
                             // CRLF style
                             || (i < textLength - 3
                                 && escaped[i + 1] == '\r'
                                 && escaped[i + 2] == '\n'
                                 && escaped[i + 3] is '\r' or '\n')))
                {
                    // CRLF style
                    if (escaped[i + 1] == '\r' && escaped[i + 2] == '\n')
                    {
                        result.Append("\r\n");
                        lineEnding = "\r\n";
                    }
                    // CR or LF style
                    else
                    {
                        result.Append(escaped[i + 1]);
                        lineEnding = escaped[i + 1].ToString();
                    }
                }
            }
        }

        return result.ToString();
    }

    /// <summary> Returns a markdown-formatted string with underline formatting. </summary>
    /// <param name="text">The text to format.</param>
    /// <param name="sanitize">Whether to sanitize the text.</param>
    /// <returns>Gets the formatted underlined text.</returns>
    /// <remarks>
    ///     Set <paramref name="sanitize"/> to <c>true</c> will sanitize the text by replacing all occurrences of
    ///     <c>(</c> and <c>)</c> with <c>\(</c> and <c>\)</c>.
    /// </remarks>
    public static string Underline(this string text, bool sanitize = true) =>
        $"(ins){(sanitize ? text.Sanitize("(", ")") : text)}(ins)";

    /// <summary> Returns a string with spoiler formatting. </summary>
    /// <param name="text">The text to format.</param>
    /// <param name="sanitize">Whether to sanitize the text.</param>
    /// <returns>Gets the formatted spoiled text.</returns>
    /// <remarks>
    ///     Set <paramref name="sanitize"/> to <c>true</c> will sanitize the text by replacing all occurrences of
    ///     <c>(</c> and <c>)</c> with <c>\(</c> and <c>\)</c>.
    /// </remarks>
    public static string Spoiler(this string text, bool sanitize = true) =>
        $"(spl){(sanitize ? text.Sanitize("(", ")") : text)}(spl)";

    /// <summary> Returns a markdown-formatted string with inline code or code block formatting. </summary>
    /// <param name="text">The text to format.</param>
    /// <param name="language"> The language of the code block. </param>
    /// <param name="sanitize">Whether to sanitize the text.</param>
    /// <returns>Gets the formatted inline code or code block.</returns>
    /// <remarks>
    ///     Set <paramref name="sanitize"/> to <c>true</c> will sanitize the text by replacing all occurrences of
    ///     <c>`</c> with <c>\`</c>.
    /// </remarks>
    public static string Code(this string text, string language = null, bool sanitize = true)
    {
        string newLine = null;
        int length = text.Length;
        for (int i = 0; i < length; i++)
            if (text[i] is '\r' or '\n')
            {
                if (text[i] == '\r' && i < length - 1 && text[i + 1] == '\n')
                    newLine = "\r\n";
                else
                    newLine = text[i].ToString();
                break;
            }

        if (language != null || newLine != null)
            return $"```{language ?? ""}{newLine ?? "\n"}{(sanitize ? text.Sanitize("`") : text)}{newLine ?? "\n"}```";
        return $"`{(sanitize ? text.Sanitize("`") : text)}`";
    }

    /// <summary> Returns a markdown-formatted string with code block formatting. </summary>
    /// <param name="text">The text to format.</param>
    /// <param name="language"> The language of the code block. </param>
    /// <param name="sanitize">Whether to sanitize the text.</param>
    /// <returns>Gets the formatted code block.</returns>
    /// <remarks>
    ///     Set <paramref name="sanitize"/> to <c>true</c> will sanitize the text by replacing all occurrences of
    ///     <c>`</c> with <c>\`</c>.
    /// </remarks>
    public static string CodeBlock(this string text, string language = null, bool sanitize = true)
    {
        string newLine = null;
        int length = text.Length;
        for (int i = 0; i < length; i++)
            if (text[i] is '\r' or '\n')
            {
                if (text[i] == '\r' && i < length - 1 && text[i + 1] == '\n')
                    newLine = "\r\n";
                else
                    newLine = text[i].ToString();
                break;
            }

        return $"```{language ?? ""}{newLine ?? "\n"}{(sanitize ? text.Sanitize("`") : text)}{newLine ?? "\n"}```";
    }


    /// <summary>
    ///     Formats a string as a block quote as a whole.
    /// </summary>
    /// <param name="text">The text to format.</param>
    /// <param name="sanitize">Whether to sanitize the text.</param>
    /// <returns>Gets the formatted block quote text.</returns>
    /// <remarks>
    ///     <note type="warning">
    ///         Due to the working mechanism of the KOOK KMarkdown renderer, this method will
    ///         insert zero-width joiner special characters (\u200d) before the first empty line
    ///         and between each empty line in the text by default, so that the renderer can
    ///         display the entire text as a single quote. When the user copies the text inside
    ///         the quote, they will not copy this special character. However, if you obtain the
    ///         message text from the server through message-related APIs, it will contain the
    ///         special character inserted by this method. If you want to disable this feature,
    ///         please use <see cref="Quote"/> instead.
    ///     </note>
    ///     <para>
    ///         Set <paramref name="sanitize"/> to <c>true</c> will sanitize the text by replacing
    ///         all occurrences of <c>&gt;</c> with <c>\&gt;</c>.
    ///     </para>
    /// </remarks>
    /// <seealso cref="Quote"/>
    public static string BlockQuote(this string text, bool sanitize = true)
    {
        string escaped = sanitize ? text.Sanitize(">") : text;

        // do not modify null or whitespace text
        // whitespace does not get quoted properly
        if (string.IsNullOrWhiteSpace(escaped)) return escaped;

        StringBuilder result = new("> ");
        for (int i = 0; i < escaped.Length; i++)
        {
            if (escaped[i] is '\n' or '\r'
                // the current is the very beginning
                && (i == 0
                    || escaped[i - 1] is '\n'
                    || (escaped[i - 1] is '\r' && escaped[i] == '\r')))
                result.Append('\u200d');
            // 插入当前字符
            result.Append(escaped[i]);
        }

        return result.ToString();
    }

    /// <summary>
    /// Remove Kook supported markdown from text.
    /// </summary>
    /// <param name="text">The to remove markdown from.</param>
    /// <returns>Gets the unformatted text.</returns>
    public static string StripMarkDown(this string text)
    {
        // Remove KOOK supported markdown
        string newText = Regex.Replace(text, @"(\*|\(ins\)|\(spl\)|`|~|>|\\)", "");

        return newText;
    }

    /// <summary>
    ///     Formats a user's username + identify number while maintaining bidirectional unicode
    /// </summary>
    /// <param name="user">The user whose username and identify number to format.</param>
    /// <param name="doBidirectional">To format the string in bidirectional unicode or not.</param>
    /// <returns>The username#identifyNumber.</returns>
    public static string UsernameAndIdentifyNumber(this IUser user, bool doBidirectional) =>
        doBidirectional
            ? $"\u2066{user.Username}\u2069#{user.IdentifyNumber}"
            : $"{user.Username}#{user.IdentifyNumber}";
}
