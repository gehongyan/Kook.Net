using System.Text.Json;

namespace Kook;

/// <summary>
///     提供用于修改 <see cref="Kook.IMessageTemplate"/> 的属性。
/// </summary>
/// <seealso cref="Kook.IMessageTemplate.ModifyAsync(System.Action{Kook.MessageTemplateProperties},Kook.RequestOptions)"/>
public class MessageTemplateProperties
{
    /// <summary>
    ///     获取或设置要设置到此消息模板标题。
    /// </summary>
    /// <remarks>
    ///     修改此值为非空字符串可以修改消息模板标题；不修改此值或将其设置为 <c>null</c> 可以保持消息模板的原标题。
    /// </remarks>
    public string? Title { get; set; }

    /// <summary>
    ///     获取或设置要设置到此消息模板类型。
    /// </summary>
    /// <remarks>
    ///     修改此值为非空字符串可以修改消息模板类型；不修改此值或将其设置为 <c>null</c> 可以保持消息模板的原类型。
    /// </remarks>
    public TemplateType? Type { get; set; }

    /// <summary>
    ///     获取或设置要设置到此消息模板消息类型。
    /// </summary>
    /// <remarks>
    ///     修改此值为非空字符串可以修改消息模板消息类型；不修改此值或将其设置为 <c>null</c> 可以保持消息模板的原消息类型。
    /// </remarks>
    public TemplateMessageType? MessageType { get; set; }

    /// <summary>
    ///     获取或设置要设置到此消息模板的内容。
    /// </summary>
    /// <remarks>
    ///     修改此值为非空字符串可以修改消息模板的内容；不修改此值或将其设置为 <c>null</c> 可以保持消息模板的原内容。
    /// </remarks>
    public string? Content { get; set; }

    /// <summary>
    ///     获取或设置要设置到此消息模板的测试数据。
    /// </summary>
    /// <remarks>
    ///     修改此值为非空字符串可以修改消息模板的测试数据；不修改此值或将其设置为 <c>null</c> 可以保持消息模板的原测试数据。
    /// </remarks>
    public JsonElement? TestData { get; set; }

    /// <summary>
    ///     获取或设置要设置到此消息模板的测试频道 ID。
    /// </summary>
    /// <remarks>
    ///     修改此值为非空字符串可以修改消息模板的测试频道 ID；不修改此值或将其设置为 <c>null</c> 可以保持消息模板的原测试频道 ID。
    /// </remarks>
    public ulong? TestChannelId { get; set; }
}
