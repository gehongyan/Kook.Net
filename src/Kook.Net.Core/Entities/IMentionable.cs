namespace Kook;

/// <summary>
///     Determines whether the object is mentionable or not.
/// </summary>
public interface IMentionable
{
    /// <summary>
    ///     Returns a special string used to mention this object in plain text formatted text.
    /// </summary>
    /// <returns>
    ///     A string that is recognized by Kook as a mention in plain text formatted text.
    /// </returns>
    string PlainTextMention { get; }

    /// <summary>
    ///     Returns a special string used to mention this object in KMarkdown formatted text.
    /// </summary>
    /// <returns>
    ///     A string that is recognized by Kook as a mention in KMarkdown formatted text.
    /// </returns>
    string KMarkdownMention { get; }
}
