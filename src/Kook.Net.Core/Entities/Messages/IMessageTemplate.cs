using System.Text.Json;

namespace Kook;

/// <summary>
///     表示一个通用的消息模板实体。
/// </summary>
public interface IMessageTemplate : IEntity<ulong>
{
    /// <summary>
    ///     获取消息模板的标题。
    /// </summary>
    string Title { get; }

    /// <summary>
    ///     获取消息模板的类型。
    /// </summary>
    TemplateType Type { get; }

    /// <summary>
    ///     获取消息模板的消息类型。
    /// </summary>
    TemplateMessageType MessageType { get; }

    /// <summary>
    ///     获取消息模板的内容。
    /// </summary>
    string Content { get; }

    /// <summary>
    ///     获取消息模板的审核状态。
    /// </summary>
    TemplateAuditStatus AuditStatus { get; }

    /// <summary>
    ///     获取消息模板的测试数据。
    /// </summary>
    JsonElement? TestData { get; }

    /// <summary>
    ///     获取消息模板的测试频道 ID。
    /// </summary>
    ulong? TestChannelId { get; }

    /// <summary>
    ///     修改此模板
    /// </summary>
    /// <param name="func"> 一个包含修改模板属性的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。 </returns>
    /// <seealso cref="Kook.MessageTemplateProperties"/>
    Task ModifyAsync(Action<MessageTemplateProperties> func, RequestOptions? options = null);

    /// <summary>
    ///     删除此模板
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步删除操作的任务。 </returns>
    Task DeleteAsync(RequestOptions? options = null);
}
