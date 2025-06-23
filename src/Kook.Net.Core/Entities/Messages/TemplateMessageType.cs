namespace Kook;

/// <summary>
///     表示消息模板的消息类型。
/// </summary>
public enum TemplateMessageType
{
    /// <summary>
    ///     KMarkdown 消息类型。
    /// </summary>
    KMarkdown = 1,

    /// <summary>
    ///     JSON 表示的卡片消息。
    /// </summary>
    JsonCard = 2,

    /// <summary>
    ///     YAML 表示的卡片消息。
    /// </summary>
    YamlCard = 3
}
