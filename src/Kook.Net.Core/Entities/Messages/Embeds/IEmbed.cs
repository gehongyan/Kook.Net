namespace Kook;

/// <summary>
///     表示一个消息内解析出的通用的嵌入式内容。
/// </summary>
/// <seealso cref="P:Kook.IMessage.Embeds"/>
public interface IEmbed
{
    /// <summary>
    ///     获取此嵌入式内容的类型。
    /// </summary>
    EmbedType Type { get; }
}
