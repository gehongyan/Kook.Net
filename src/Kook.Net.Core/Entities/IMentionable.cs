namespace Kook;

/// <summary>
///     表示一个可以被提及的实体对象。
/// </summary>
public interface IMentionable
{
    /// <summary>
    ///     获取一个用于在纯文本格式文本中提及此对象的格式化字符串。
    /// </summary>
    string PlainTextMention { get; }

    /// <summary>
    ///     返回一个用于在 KMarkdown 格式文本中提及此对象的格式化字符串。
    /// </summary>
    string KMarkdownMention { get; }
}
