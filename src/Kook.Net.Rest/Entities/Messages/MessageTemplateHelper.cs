using System.Text.Json;
using Kook.API;
using Kook.API.Rest;

namespace Kook.Rest;

internal static class MessageTemplateHelper
{
    public static async Task<IReadOnlyCollection<RestMessageTemplate>> GetMessageTemplatesAsync(
        BaseKookClient client, RequestOptions? options)
    {
        IEnumerable<MessageTemplate> models =
            await client.ApiClient.GetMessageTemplatesAsync(options: options).FlattenAsync().ConfigureAwait(false);
        return [..models.Select(x => RestMessageTemplate.Create(client, x))];
    }

    public static async Task<RestMessageTemplate> GetMessageTemplateAsync(
        BaseKookClient client, ulong id,RequestOptions? options)
    {
        MessageTemplate model = await client.ApiClient.GetMessageTemplateAsync(id, options).ConfigureAwait(false);
        return RestMessageTemplate.Create(client, model);
    }

    public static async Task<RestMessageTemplate> CreateAsync(BaseKookClient client, string title, string content,
        TemplateType type = TemplateType.Twig, TemplateMessageType messageType = TemplateMessageType.KMarkdown,
        ulong? testChannelId = null, JsonElement? testData = null)
    {
        CreateMessageTemplateParams args = new()
        {
            Title = title,
            Type = type,
            MessageType = messageType,
            Content = content,
            TestChannel = testChannelId?.ToString(),
            TestData = testData?.GetRawText()
        };
        CreateOrModifyMessageTemplateResponse response = await client.ApiClient
            .CreateMessageTemplateAsync(args).ConfigureAwait(false);
        return RestMessageTemplate.Create(client, response.Model);
    }

    public static async Task ModifyAsync(RestMessageTemplate template, BaseKookClient client,
        Action<MessageTemplateProperties> func, RequestOptions? options)
    {
        MessageTemplateProperties properties = new()
        {
            Title = template.Title,
            Type = template.Type,
            MessageType = template.MessageType,
            Content = template.Content,
            TestData = template.TestData,
            TestChannelId = template.TestChannelId
        };
        func(properties);
        ModifyMessageTemplateParams args = new()
        {
            Id = template.Id,
            Title = properties.Title,
            Type = properties.Type,
            MessageType = properties.MessageType,
            Content = properties.Content,
            TestChannel = properties.TestChannelId?.ToString(),
            TestData = properties.TestData?.GetRawText()
        };
        CreateOrModifyMessageTemplateResponse response = await client.ApiClient
            .ModifyMessageTemplateAsync(args, options).ConfigureAwait(false);
        template.Update(response.Model);
    }

    public static async Task DeleteAsync(RestMessageTemplate template, BaseKookClient client, RequestOptions? options)
    {
        DeleteMessageTemplateParams args = new()
        {
            Id = template.Id
        };
        await client.ApiClient.DeleteMessageTemplateAsync(args, options).ConfigureAwait(false);
    }
}
