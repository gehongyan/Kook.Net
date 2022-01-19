using System.Text;
using System.Text.RegularExpressions;

namespace KaiHeiLa;

public static class KMarkdownFormat
{
    public static string Bold(string text) => $"**{text}**";
    
    public static string Italics(string text) => $"*{text}*";
    
    public static string Strikethrough(string text) => $"~~{text}~~";
    
    public static string Url(string text, string url) => $"[{text}]({url})";

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
}