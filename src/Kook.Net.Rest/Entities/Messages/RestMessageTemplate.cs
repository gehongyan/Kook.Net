using System.Diagnostics;
using System.Text.Json;
using Kook.API;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的消息模板。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestMessageTemplate : RestEntity<ulong>, IMessageTemplate, IUpdateable
{
    /// <inheritdoc />
    public string Title { get; private set; }

    /// <inheritdoc />
    public TemplateType Type { get; private set; }

    /// <inheritdoc />
    public TemplateMessageType MessageType { get; private set; }

    /// <inheritdoc />
    public string Content { get; private set; }

    /// <inheritdoc />
    public TemplateAuditStatus AuditStatus { get; private set; }

    /// <inheritdoc />
    public JsonElement? TestData { get; private set; }

    /// <inheritdoc />
    public ulong? TestChannelId { get; private set; }

    internal RestMessageTemplate(BaseKookClient kook, ulong id)
        : base(kook, id)
    {
        Title = string.Empty;
        Content = string.Empty;
    }

    internal static RestMessageTemplate Create(BaseKookClient kook, API.MessageTemplate model)
    {
        RestMessageTemplate entity = new(kook, model.Id);
        entity.Update(model);
        return entity;
    }

    internal void Update(API.MessageTemplate model)
    {
        Title = model.Title;
        Type = model.Type;
        MessageType = model.MessageType;
        Content = model.Content;
        AuditStatus = model.Status;
        if (model.TestData != null)
        {
            try
            {
                TestData = JsonDocument.Parse(model.TestData).RootElement;
            }
            catch
            {
                TestData = null;
            }
        }
        TestChannelId = ulong.TryParse(model.TestChannel, out ulong testChannelId) ? testChannelId : null;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(RequestOptions? options = null)
    {
        MessageTemplate? model = await Kook.ApiClient.GetMessageTemplateAsync(Id, options);
        if (model is null)
            throw new InvalidOperationException("Failed to retrieve message template.");
        Update(model);
    }

    /// <inheritdoc />
    public Task ModifyAsync(Action<MessageTemplateProperties> func, RequestOptions? options = null) =>
        MessageTemplateHelper.ModifyAsync(this, Kook, func, options);

    /// <inheritdoc />
    public Task DeleteAsync(RequestOptions? options = null) => MessageTemplateHelper.DeleteAsync(this, Kook, options);

    private string DebuggerDisplay => $"{Title} ({Id}, {Type}, {MessageType})";
}
