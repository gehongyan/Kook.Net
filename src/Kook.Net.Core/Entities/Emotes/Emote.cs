using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Kook;

/// <summary>
///     表示一个表情符号。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Emote : IEmote
{
    internal static readonly Regex PlainTextEmojiRegex = new(@"\[:(?<name>[^:]{1,32}?):(?<id>[\w\/]{1,40}?)\]",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    internal static readonly Regex KMarkdownEmojiRegex = new(@"(\(emj\))(?<name>[^\(\)]{1,32}?)\1\[(?<id>[\w\/]{1,40}?)\]",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    /// <summary>
    ///     获取此表情符号的唯一标识符。
    /// </summary>
    public string Id { get; }

    /// <summary>
    ///     获取此表情符号的名称。
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     获取此表情符号是否为动态表情。如果无法确定此表情符号是否为动态表情，则为 <c>null</c>。
    /// </summary>
    public bool? Animated { get; }

    /// <summary>
    ///     创建一个新的 <see cref="T:Kook.Emote" /> 实例。
    /// </summary>
    public Emote(string id, string name, bool? animated = null)
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

    /// <summary>
    ///     尝试从一个表情符号的原始格式中解析出一个 <see cref="T:Kook.Emote"/>。
    /// </summary>
    /// <param name="text">
    ///     表情符号的原始格式。例如：<paramref name="tagMode"/> 为 <see cref="F:Kook.TagMode.PlainText"/> 时的
    ///     <c>[:emotename:1991895624896587/hbCFVWhu923k03k]</c>；为 <see cref="F:Kook.TagMode.KMarkdown"/> 时的
    ///     <c>(emj)emotename(emj)[1991895624896587/hbCFVWhu923k03k]</c>。
    /// </param>
    /// <param name="result"> 如果解析成功，则为解析出的 <see cref="T:Kook.Emote"/>；否则为 <c>null</c>。 </param>
    /// <param name="tagMode"> 解析标签的语法模式。 </param>
    /// <returns> 如果解析成功，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool TryParse([NotNullWhen(true)] string? text,
        [NotNullWhen(true)] out Emote? result, TagMode tagMode)
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

    /// <summary>
    ///     从一个表情符号的原始格式中解析出一个 <see cref="T:Kook.Emote"/>。
    /// </summary>
    /// <param name="text">
    ///     表情符号的原始格式。例如：<paramref name="tagMode"/> 为 <see cref="F:Kook.TagMode.PlainText"/> 时的
    ///     <c>[:emotename:1991895624896587/hbCFVWhu923k03k]</c>；为 <see cref="F:Kook.TagMode.KMarkdown"/> 时的
    ///     <c>(emj)emotename(emj)[1991895624896587/hbCFVWhu923k03k]</c>。
    /// </param>
    /// <param name="tagMode"> 解析标签的语法模式。 </param>
    /// <returns> 解析出的 <see cref="T:Kook.Emote"/>。 </returns>
    /// <exception cref="ArgumentException">
    ///     无法以 <paramref name="tagMode"/> 的语法模式解析 <paramref name="text"/> 为一个有效的表情符号。
    /// </exception>
    public static Emote Parse([NotNull] string? text, TagMode tagMode)
    {
        if (TryParse(text, out Emote? result, tagMode))
            return result;
        throw new FormatException("Invalid emote format.");
    }

    /// <inheritdoc />
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    ///     获取此表情的 KMarkdown 格式字符串。
    /// </summary>
    /// <returns> 此表情的 KMarkdown 格式字符串。 </returns>
    public string ToKMarkdownString() => $"(emj){Name}(emj)[{Id}]";

    /// <summary>
    ///     获取此表情的纯文本格式字符串。
    /// </summary>
    /// <returns> 此表情的纯文本格式字符串。 </returns>
    public string ToPlainTextString() => $"[:{Name}:{Id}]";

    private string DebuggerDisplay => $"{Name} ({Id})";

    /// <inheritdoc cref="M:Kook.Emote.ToKMarkdownString" />
    public override string ToString() => ToString(TagMode.KMarkdown);

    /// <summary>
    ///     获取此表情的字符串表示形式。
    /// </summary>
    /// <param name="tagMode"> 标签的语法模式。 </param>
    /// <returns> 此表情的字符串表示形式。 </returns>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="tagMode"/> 不是有效的标签语法模式。 </exception>
    public string ToString(TagMode tagMode) => tagMode switch
    {
        TagMode.PlainText => ToPlainTextString(),
        TagMode.KMarkdown => ToKMarkdownString(),
        _ => throw new ArgumentOutOfRangeException(nameof(tagMode), tagMode, null)
    };
}
