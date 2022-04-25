using System.Text;
using System.Text.RegularExpressions;

namespace KaiHeiLa;

public static class Format
{
    private static readonly string[] SensitiveCharacters = {
        "\\", "*", "~", "`", ":", "-", "]", ")", ">" };

    private static readonly Regex MarkdownEscapeRegex = new Regex(

        @"((?<!\\)([*]{1,3}|~{2}|`(``\w*\n)?|\((spl|ins)\)))" +         // Unescaped asterisks, tildes, backticks, (spl) and (ins)
        @"(\\(?=[\*~`\(\)\[\]\\]))" +                                       // All escaping backslashes
        @"((?<=((?<!\\)\[).*?[^\\])\]\(https?://.*?\))" +                   // Tales of links
        @"((?<!\\)(?=.*?[^\\]\]\(https?://.*?\))\[)" +                      // Open bracket of links
        @"(((?<=\n)\n)?^>\s)"                                               // Quotes and related redundant newlines
    );
    
    public static string Bold(string text) => $"**{text}**";
    
    public static string Italics(string text) => $"*{text}*";
    
    public static string Strikethrough(string text) => $"~~{text}~~";
    
    public static string Url(string text, string url) => $"[{text}]({url})";

    /// <summary> Sanitizes the string, safely escaping any Markdown sequences. </summary>
    public static string Sanitize(string text)
    {
        return SensitiveCharacters.Aggregate(text,
            (current, unsafeChar) => current.Replace(unsafeChar, $"\\{unsafeChar}"));
    }
    
    public static string Quote(string text)
    {
        // do not modify null or whitespace text
        // whitespace does not get quoted properly
        if (string.IsNullOrWhiteSpace(text))
            return text;

        StringBuilder result = new StringBuilder();

        int startIndex = 0;
        int newLineIndex;
        do
        {
            newLineIndex = text.IndexOf('\n', startIndex);
            if (newLineIndex == -1)
            {
                // read the rest of the string
                var str = text.Substring(startIndex);
                result.Append($"> {str}");
            }
            else
            {
                // read until the next newline
                var str = text.Substring(startIndex, newLineIndex - startIndex);
                result.Append($"> {str}\n");
            }
            startIndex = newLineIndex + 1;
        }
        while (newLineIndex != -1 && startIndex != text.Length);

        return result.ToString();
    }
    
    public static string Underline(string text) => $"(ins){text}(ins)";
    
    public static string Spoiler(string text) => $"(spl){text}(spl)";
    
    public static string Code(string text, string language = null)
    {
        if (language != null || text.Contains('\n'))
            return $"```{language ?? ""}\n{text}\n```";
        else
            return $"`{text}`";
    }
    
    /// <summary>
    /// Remove KaiHeiLa supported markdown from text.
    /// </summary>
    /// <param name="text">The to remove markdown from.</param>
    /// <returns>Gets the unformatted text.</returns>
    public static string StripMarkDown(string text)
    {
        //Remove KaiHeiLa supported markdown
        var newText = MarkdownEscapeRegex.Replace(text, ""); // @"(\*|\(ins\)|\(spl\)|`|~|>|\\)"
        return newText;
    }
    
    /// <summary>
    ///     Formats a user's username + discriminator while maintaining bidirectional unicode
    /// </summary>
    /// <param name="user">The user whos username and discriminator to format</param>
    /// <returns>The username + discriminator</returns>
    public static string UsernameAndIdentifyNumber(IUser user)
    {
        return $"\u2066{user.Username}\u2069#{user.IdentifyNumber}";
    }
}