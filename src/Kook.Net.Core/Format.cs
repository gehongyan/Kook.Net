using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace Kook;

/// <summary>
///     提供用于格式化字符的帮助类。
/// </summary>
public static class Format
{
    private static readonly string[] SensitiveCharacters = ["\\", "*", "~", "`", ":", "-", "]", ")", ">"];

    /// <summary>
    ///     返回一个使用粗体格式的 KMarkdown 格式化字符串。
    /// </summary>
    /// <param name="text"> 要格式化的文本。 </param>
    /// <param name="sanitize"> 是否要先对 <paramref name="text"/> 中与当前格式化操作有冲突的字符进行转义。 </param>
    /// <returns> 获取格式化后的文本。 </returns>
    /// <remarks>
    ///     设置 <paramref name="sanitize"/> 为 <c>true</c> 将会对文本中出现的所有 <c>*</c> 字符转义为 <c>\*</c>。
    /// </remarks>
    public static string Bold(this string? text, bool sanitize = true) =>
        $"**{(sanitize ? text.Sanitize("*") : text)}**";

    /// <summary>
    ///     返回一个使用斜体格式的 KMarkdown 格式化字符串。
    /// </summary>
    /// <param name="text"> 要格式化的文本。 </param>
    /// <param name="sanitize"> 是否要先对 <paramref name="text"/> 中与当前格式化操作有冲突的字符进行转义。 </param>
    /// <returns> 获取格式化后的文本。 </returns>
    /// <remarks>
    ///     设置 <paramref name="sanitize"/> 为 <c>true</c> 将会对文本中出现的所有 <c>*</c> 字符转义为 <c>\*</c>。
    /// </remarks>
    public static string Italics(this string? text, bool sanitize = true) =>
        $"*{(sanitize ? text.Sanitize("*") : text)}*";

    /// <summary>
    ///     返回一个使用粗斜体格式的 KMarkdown 格式化字符串。
    /// </summary>
    /// <param name="text"> 要格式化的文本。 </param>
    /// <param name="sanitize"> 是否要先对 <paramref name="text"/> 中与当前格式化操作有冲突的字符进行转义。 </param>
    /// <returns> 获取格式化后的文本。 </returns>
    /// <remarks>
    ///     设置 <paramref name="sanitize"/> 为 <c>true</c> 将会对文本中出现的所有 <c>*</c> 字符转义为 <c>\*</c>。
    /// </remarks>
    public static string BoldItalics(this string? text, bool sanitize = true) =>
        $"***{(sanitize ? text.Sanitize("*") : text)}***";

    /// <summary>
    ///     返回一个使用删除线格式的 KMarkdown 格式化字符串。
    /// </summary>
    /// <param name="text"> 要格式化的文本。 </param>
    /// <param name="sanitize"> 是否要先对 <paramref name="text"/> 中与当前格式化操作有冲突的字符进行转义。 </param>
    /// <returns> 获取格式化后的文本。 </returns>
    /// <remarks>
    ///     设置 <paramref name="sanitize"/> 为 <c>true</c> 将会对文本中出现的所有 <c>~</c> 字符转义为 <c>\~</c>。
    /// </remarks>
    public static string Strikethrough(this string? text, bool sanitize = true) =>
        $"~~{(sanitize ? text.Sanitize("~") : text)}~~";

    /// <summary>
    ///     返回一个使用彩色文本格式的 KMarkdown 格式化字符串。
    /// </summary>
    /// <param name="text"> 要格式化的文本。 </param>
    /// <param name="theme"> 要应用的文本颜色。 </param>
    /// <param name="sanitize"> 是否要先对 <paramref name="text"/> 中与当前格式化操作有冲突的字符进行转义。 </param>
    /// <returns> 获取格式化后的文本。 </returns>
    /// <remarks>
    ///     <note type="warning">
    ///         目前，KOOK 仅支持在卡片消息中使用 KMarkdown 彩色文本格式。
    ///     </note>
    /// </remarks>
    /// <remarks>
    ///     设置 <paramref name="sanitize"/> 为 <c>true</c> 将会对文本中出现的所有 <c>(</c> 和 <c>)</c>
    ///     字符转分别义为 <c>\(</c> 和 <c>\)</c>。
    /// </remarks>
    public static string Colorize(this string? text, TextTheme theme, bool sanitize = true) =>
        $"(font){(sanitize ? text.Sanitize("(", ")") : text)}(font)[{theme.ToString().ToLowerInvariant()}]";

    /// <summary>
    ///     返回格式化后的 KMarkdown 链接。
    /// </summary>
    /// <param name="text"> 要格式化的文本。 </param>
    /// <param name="url"> 要链接到的 URL。 </param>
    /// <param name="sanitize"> 是否要先对 <paramref name="text"/> 与 <paramref name="url"/> 中与当前格式化操作有冲突的字符进行转义。 </param>
    /// <returns> 获取格式化后的链接文本。 </returns>
    /// <remarks>
    ///     设置 <paramref name="sanitize"/> 为 <c>true</c> 将会对文本中出现的所有 <c>[</c> 和 <c>]</c> 字符分别转义为
    ///     <c>\[</c> 和 <c>\]</c>，并对 URL 中出现的所有 <c>(</c> 和 <c>)</c> 字符分别转义为 <c>\(</c> 和 <c>\)</c>。
    /// </remarks>
    public static string Url(this string? text, string url, bool sanitize = true) =>
        $"[{(sanitize ? text.Sanitize("[", "]") : text)}]({(sanitize ? url.Sanitize("(", ")") : url)})";

    /// <inheritdoc cref="M:Kook.Format.Url(System.String,System.String,System.Boolean)" />
    public static string Url(this string? text, Uri url, bool sanitize = true) =>
        text.Url(url.ToString(), sanitize);

    /// <summary>
    ///     转义字符串，安全地转义任何 KMarkdown 序列。
    /// </summary>
    /// <param name="text"> 要转义的文本。 </param>
    /// <param name="sensitiveCharacters"> 要转义的字符。 </param>
    /// <returns> 获取转义后的文本。 </returns>
    /// <remarks>
    ///     如果未指定要转移的字符，则将使用默认的转义字符列表。默认的待转义字符包括：<br />
    ///     <c>\</c>、<c>*</c>、<c>~</c>、<c>`</c>、<c>:</c>、<c>-</c>、<c>]</c>、<c>)</c>、<c>&gt;</c>。
    /// </remarks>
    [return: NotNullIfNotNull(nameof(text))]
    public static string? Sanitize(this string? text, params string[] sensitiveCharacters)
    {
        if (text is null) return null;
        string[] sensitiveChars = sensitiveCharacters.Length > 0 ? sensitiveCharacters : SensitiveCharacters;
        return sensitiveChars.Aggregate(text,
            (current, unsafeChar) => current.Replace(unsafeChar, $"\\{unsafeChar}"));
    }

    /// <summary>
    ///     将字符串格式化为由多个换行符分隔的引用字符串。
    /// </summary>
    /// <param name="text"> 要格式化的文本。 </param>
    /// <param name="sanitize"> 是否要先对 <paramref name="text"/> 中与当前格式化操作有冲突的字符进行转义。 </param>
    /// <returns> 获取格式化后的引用文本。 </returns>
    /// <remarks>
    ///     此方法尽可能地保持字符串在 KOOK 中的渲染排版，保留换行与空格，各个文本块会被分别引用。 <br />
    ///     <note type="warning">
    ///         此方法会将两个或更多连续的换行符识别多个独立文本块，并将每个块分别格式化为引用块。
    ///         对于每个文本块，在字符串的开头插入一个大于号 (<c>&gt;</c>) 和一个空格；如果块的开头是一个空白字符，则还会插入一个
    ///         零宽连接符 (<c>\u200d</c>)，以确保引用的正确显示。当用户复制引用内的文本时，他们不会复制到这个特殊字符。
    ///         但是，如果通过消息相关的 API 从服务器获取消息文本，返回的消息文本将包含此方法插入的特殊字符。
    ///         另外，在块的末尾还会附加一个额外的换行符，以修正由于引用格式化引起的换行缺失问题。附加的换行符与附近的换行符样式一致。
    ///         如果要引用整个文本块，请使用 <see cref="M:Kook.Format.BlockQuote(System.String,System.Boolean)"/>。
    ///     </note>
    ///     <br />
    ///     此方法会尝试分析字符串中的行分隔符，并在需要因要保持排版时插入行分隔符时，尽可能地使用原有的行分隔符。 <br />
    ///     设置 <paramref name="sanitize"/> 为 <c>true</c> 将会对文本中出现的所有 <c>&gt;</c> 字符转义为 <c>\&gt;</c>。
    /// </remarks>
    /// <seealso cref="M:Kook.Format.BlockQuote(System.String,System.Boolean)"/>
    [return: NotNullIfNotNull(nameof(text))]
    public static string? Quote(this string? text, bool sanitize = true)
    {
        string? escaped = sanitize ? text.Sanitize(">") : text;

        // do not modify null or whitespace text
        // whitespace does not get quoted properly
        if (escaped is null || string.IsNullOrWhiteSpace(escaped))
            return escaped;

        StringBuilder result = new();

        string? lineEnding = null;
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

    /// <summary>
    ///     返回一个使用下划线格式的 KMarkdown 格式化字符串。
    /// </summary>
    /// <param name="text"> 要格式化的文本。 </param>
    /// <param name="sanitize"> 是否要先对 <paramref name="text"/> 中与当前格式化操作有冲突的字符进行转义。 </param>
    /// <returns> 获取格式化后的文本。 </returns>
    /// <remarks>
    ///     设置 <paramref name="sanitize"/> 为 <c>true</c> 将会对文本中出现的所有 <c>(</c> 和 <c>)</c> 字符分别转义为
    ///     <c>\(</c> 和 <c>\)</c>。
    /// </remarks>
    public static string Underline(this string? text, bool sanitize = true) =>
        $"(ins){(sanitize ? text.Sanitize("(", ")") : text)}(ins)";

    /// <summary>
    ///     返回一个使用剧透格式的 KMarkdown 格式化字符串。
    /// </summary>
    /// <param name="text"> 要格式化的文本。 </param>
    /// <param name="sanitize"> 是否要先对 <paramref name="text"/> 中与当前格式化操作有冲突的字符进行转义。 </param>
    /// <returns> 获取格式化后的文本。 </returns>
    /// <remarks>
    ///     设置 <paramref name="sanitize"/> 为 <c>true</c> 将会对文本中出现的所有 <c>(</c> 和 <c>)</c> 字符分别转义为
    ///     <c>\(</c> 和 <c>\)</c>。
    /// </remarks>
    public static string Spoiler(this string? text, bool sanitize = true) =>
        $"(spl){(sanitize ? text.Sanitize("(", ")") : text)}(spl)";

    /// <summary>
    ///     返回一个使用代码格式的 KMarkdown 格式化字符串。
    /// </summary>
    /// <param name="text"> 要格式化的文本。 </param>
    /// <param name="language"> 代码块的语言。 </param>
    /// <param name="sanitize"> 是否要先对 <paramref name="text"/> 中与当前格式化操作有冲突的字符进行转义。 </param>
    /// <returns> 获取格式化后的内联代码或代码块。 </returns>
    /// <remarks>
    ///     设置 <paramref name="sanitize"/> 为 <c>true</c> 将会对文本中出现的所有 <c>`</c> 字符转义为 <c>\`</c>。 <br />
    ///     当 <paramref name="language"/> 不为 <c>null</c> 或 <paramref name="text"/> 中包含换行符时，将返回一个代码块；
    ///     否则，将返回一个内联代码块。
    /// </remarks>
    public static string Code(this string? text, string? language = null, bool sanitize = true)
    {
        if (text is null)
            return "`\u200d`";

        string? newLine = null;
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

    /// <summary>
    ///     返回一个使用代码块格式的 KMarkdown 格式化字符串。
    /// </summary>
    /// <param name="text"> 要格式化的文本。 </param>
    /// <param name="language"> 代码块的语言。 </param>
    /// <param name="sanitize"> 是否要先对 <paramref name="text"/> 中与当前格式化操作有冲突的字符进行转义。 </param>
    /// <returns> 获取格式化后的代码块。 </returns>
    /// <remarks>
    ///     设置 <paramref name="sanitize"/> 为 <c>true</c> 将会对文本中出现的所有 <c>`</c> 字符转义为 <c>\`</c>。
    /// </remarks>
    public static string CodeBlock(this string? text, string? language = null, bool sanitize = true)
    {
        if (text is null) return "```\n\n```";
        string? newLine = null;
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
    ///     返回一个使用引用块格式的 KMarkdown 格式化字符串。
    /// </summary>
    /// <param name="text"> 要格式化的文本。 </param>
    /// <param name="sanitize"> 是否要先对 <paramref name="text"/> 中与当前格式化操作有冲突的字符进行转义。 </param>
    /// <returns> 获取格式化后的引用文本。 </returns>
    /// <remarks>
    ///     <note type="warning">
    ///         此方法默认会在文本中的第一个空行之前和每个空行之间插入零宽连接符特殊字符 (\u200d)，以便渲染器能够将整个文本显示为单个引用。
    ///         当用户复制引用内的文本时，他们不会复制这个特殊字符。但是，如果通过消息相关的 API 从服务器获取消息文本，
    ///         它将包含此方法插入的特殊字符。
    ///     </note>
    ///     <br />
    ///     设置 <paramref name="sanitize"/> 为 <c>true</c> 将会对文本中出现的所有 <c>&gt;</c> 字符转义为 <c>\&gt;</c>。
    /// </remarks>
    /// <seealso cref="M:Kook.Format.Quote(System.String,System.Boolean)"/>
    public static string BlockQuote(this string? text, bool sanitize = true)
    {
        if (text is null) return "> \u200d";

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

    /// <inheritdoc cref="M:Kook.Format.StripMarkdown(System.String)" />
    [Obsolete("Use StripMarkdown instead.")]
    public static string StripMarkDown(this string text) => StripMarkdown(text);

    /// <summary>
    ///     移除文本中的 KMarkdown 格式字符。
    /// </summary>
    /// <param name="text"> 要移除 KMarkdown 格式字符的文本。 </param>
    /// <returns> 获取移除 KMarkdown 格式字符后的文本。 </returns>
    /// <remarks>
    ///     此方法不会过多地分析 KMarkdown 的复杂格式，只会简单地移除 KMarkdown 中的以下字符：<br />
    ///     <c>*</c>、<c>(ins)</c>、<c>(spl)</c>、<c>`</c>、<c>~</c>、<c>&gt;</c>、<c>\</c>、连续两个或更多地 <c>-</c>。
    /// </remarks>
    public static string StripMarkdown(this string text) =>
        // Remove KOOK supported markdown
        Regex.Replace(text, @"\*|\(ins\)|\(spl\)|`|~|>|\\|-{2,}", "", RegexOptions.Compiled);

    /// <summary>
    ///     保持双向 Unicode 格式化的情况下格式化用户的用户名称 + 识别号。
    /// </summary>
    /// <param name="user"> 要格式化其用户名称与识别号的用户。 </param>
    /// <param name="doBidirectional"> 是否要保持双向 Unicode 进行格式化。 </param>
    /// <returns> 获取格式化后的用户名称与识别号。 </returns>
    /// <seealso cref="P:Kook.KookConfig.FormatUsersInBidirectionalUnicode"/>
    public static string UsernameAndIdentifyNumber(this IUser user, bool doBidirectional) =>
        doBidirectional
            ? $"\u2066{user.Username}\u2069#{user.IdentifyNumber}"
            : $"{user.Username}#{user.IdentifyNumber}";
}
