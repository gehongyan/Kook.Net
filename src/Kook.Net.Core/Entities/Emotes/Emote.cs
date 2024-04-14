using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Kook;

/// <summary>
///     Represents a guild emote.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Emote : IEmote
{
    internal static readonly Regex PlainTextEmojiRegex = new(@"\[:(?<name>[^:]{1,32}?):(?<id>[\w\/]{1,40}?)\]",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    internal static readonly Regex KMarkdownEmojiRegex = new(@"(\(emj\))(?<name>[^\(\)]{1,32}?)\1\[(?<id>[\w\/]{1,40}?)\]",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    /// <summary>
    ///     Gets the identifier of this emote.
    /// </summary>
    public string Id { get; }

    /// <summary>
    ///     Gets the name of this emote.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets whether this emote is animated.
    /// </summary>
    public bool? Animated { get; }

    internal Emote(string id, string name, bool? animated = null)
    {
        Id = id;
        Name = name;
        Animated = animated;
    }

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj == null) return false;
        if (obj == this) return true;
        if (obj is not Emote otherEmote) return false;
        return Id == otherEmote.Id;
    }

    /// <summary> Tries to parse an <see cref="Emote"/> from its raw format. </summary>
    /// <param name="text">
    ///     The raw encoding of an emote; for example,
    ///     [:emotename:1991895624896587/hbCFVWhu923k03k] when <paramref name="tagMode"/> is <c>TagMode.PlainText</c>,
    ///     or (emj)emotename(emj)[1991895624896587/hbCFVWhu923k03k] when <paramref name="tagMode"/> is <c>TagMode.KMarkdown</c>.
    /// </param>
    /// <param name="result">An emote.</param>
    /// <param name="tagMode"></param>
    public static bool TryParse([NotNullWhen(true)] string? text, [NotNullWhen(true)] out Emote? result, TagMode tagMode)
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
        if (!match.Success)
            return false;
        result = new Emote(match.Groups["id"].Value, match.Groups["name"].Value);
        return true;
    }

    /// <summary> Parses an <see cref="Emote"/> from its raw format. </summary>
    /// <param name="text">
    ///     The raw encoding of an emote; for example,
    ///     [:emotename:1991895624896587/hbCFVWhu923k03k] when <paramref name="tagMode"/> is <c>TagMode.PlainText</c>,
    ///     or (emj)emotename(emj)[1991895624896587/hbCFVWhu923k03k] when <paramref name="tagMode"/> is <c>TagMode.KMarkdown</c>.
    /// </param>
    /// <param name="tagMode"></param>
    /// <returns>An emote.</returns>
    /// <exception cref="ArgumentException">Invalid emote format.</exception>
    public static Emote Parse([NotNull] string? text, TagMode tagMode)
    {
        if (TryParse(text, out Emote? result, tagMode))
            return result;
        throw new ArgumentException("Invalid emote format.", nameof(text));
    }

    /// <inheritdoc />
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    ///     Gets a string representation of the emote in KMarkdown format.
    /// </summary>
    public string ToKMarkdownString() => $"(emj){Name}(emj)[{Id}]";

    /// <summary>
    ///     Gets a string representation of the emote in plain text format.
    /// </summary>
    public string ToPlainTextString() => $"[:{Name}:{Id}]";

    private string DebuggerDisplay => $"{Name} ({Id})";

    /// <summary>
    ///     Returns the raw representation of the emote.
    /// </summary>
    /// <returns>
    ///     A string representing the raw presentation of the emote (e.g. <c>[:thonkang:282745590985523200]</c>).
    /// </returns>
    public override string ToString() => $"[:{Name}:{Id}]";
}
