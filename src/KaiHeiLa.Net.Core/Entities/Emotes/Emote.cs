using System.Globalization;
using System.Text.RegularExpressions;

namespace KaiHeiLa;

public class Emote : IEmote
{
    
    internal static readonly Regex PlainTextEmojiRegex = new Regex(@"\[:(?<name>[^:]+?):(?<id>\d{1,20}\/\w{1,20})\]",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);
    
    internal static readonly Regex KMarkdownEmojiRegex = new Regex(@"(\(emj\))(?<name>[^\(\)]{1,20}?)\1\[(?<id>\d{1,20}\/\w{1,20})\]",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);
    
    public string Id { get; }
    
    public string Name { get; }
    
    public bool? Animated { get; }
    
    internal Emote(string id, string name, bool? animated = null)
    {
        Id = id;
        Name = name;
        Animated = animated;
    }
    
    public override bool Equals(object other)
    {
        if (other == null) return false;
        if (other == this) return true;

        if (other is not Emote otherEmote) 
            return false;

        return Id == otherEmote.Id;
    }
    
    public override int GetHashCode()
        => Id.GetHashCode();
    
    /// <summary> Parses an <see cref="Emote"/> from its raw format. </summary>
    /// <param name="text">
    ///     The raw encoding of an emote; for example,
    ///     [:djbigfan:1990044438283387/hvBcVC4nHX03k03k] when <paramref name="tagMode"/> is <c>TagMode.PlainText</c>,
    ///     or (emj)djbigfan(emj)[1990044438283387/hvBcVC4nHX03k03k] when <paramref name="tagMode"/> is <c>TagMode.KMarkdown</c>.
    /// </param>
    /// <param name="tagMode"></param>
    /// <returns>An emote.</returns>
    /// <exception cref="ArgumentException">Invalid emote format.</exception>
    public static Emote Parse(string text, TagMode tagMode)
    {
        if (TryParse(text, out Emote result, tagMode))
            return result;
        throw new ArgumentException(message: "Invalid emote format.", paramName: nameof(text));
    }
    
    /// <summary> Tries to parse an <see cref="Emote"/> from its raw format. </summary>
    /// <param name="text">
    ///     The raw encoding of an emote; for example,
    ///     [:djbigfan:1990044438283387/hvBcVC4nHX03k03k] when <paramref name="tagMode"/> is <c>TagMode.PlainText</c>,
    ///     or (emj)djbigfan(emj)[1990044438283387/hvBcVC4nHX03k03k] when <paramref name="tagMode"/> is <c>TagMode.KMarkdown</c>.
    /// </param>
    /// <param name="result">An emote.</param>
    /// <param name="tagMode"></param>
    public static bool TryParse(string text, out Emote result, TagMode tagMode)
    {
        result = null;

        if (text == null)
            return false;

        Match match = tagMode switch
        {
            TagMode.PlainText => PlainTextEmojiRegex.Match(text),
            TagMode.KMarkdown => KMarkdownEmojiRegex.Match(text),
            _ => throw new ArgumentOutOfRangeException(nameof(tagMode), tagMode, null)
        };

        if (match.Success)
        {
            result = new Emote(match.Groups["id"].Value, match.Groups["name"].Value);
            return true;
        }

        return false;
    }
    
    private string DebuggerDisplay => $"{Name} ({Id})";
    /// <summary>
    ///     Returns the raw representation of the emote.
    /// </summary>
    /// <returns>
    ///     A string representing the raw presentation of the emote (e.g. <c>[:thonkang:282745590985523200]</c>).
    /// </returns>
    public override string ToString() => $"[:{Name}:{Id}]";
}