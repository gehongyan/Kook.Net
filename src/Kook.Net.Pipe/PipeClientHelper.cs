using System.Text.Json;
using Kook.API.Rest;
using Kook.Rest;

namespace Kook.Pipe;

internal static class PipeClientHelper
{
    public static async Task<Guid> SendMessageAsync<T>(KookPipeClient client, T? parameters,
        JsonSerializerOptions? jsonSerializerOptions, RequestOptions? options)
    {
        CreateMessageResponse response = await client.ApiClient
            .CreatePipeMessageAsync(parameters, jsonSerializerOptions, options).ConfigureAwait(false);
        return response.MessageId;
    }

    public static async Task<Guid> SendTextAsync(KookPipeClient client, string text, RequestOptions? options)
    {
        CreatePipeMessageParams args = new()
        {
            Content = text
        };
        CreateMessageResponse response = await client.ApiClient
            .CreatePipeMessageAsync(args, MessageType.KMarkdown, options).ConfigureAwait(false);
        return response.MessageId;
    }

    public static async Task<Guid> SendCardsAsync(KookPipeClient client, IEnumerable<ICard> cards, RequestOptions? options)
    {
        string json = MessageHelper.SerializeCards(cards);
        CreatePipeMessageParams args = new()
        {
            Content = json
        };
        CreateMessageResponse response = await client.ApiClient
            .CreatePipeMessageAsync(args, MessageType.Card, options).ConfigureAwait(false);
        return response.MessageId;
    }
}
