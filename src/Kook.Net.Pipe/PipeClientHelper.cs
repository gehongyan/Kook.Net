using System.Text.Json;
using Kook.API.Rest;
using Kook.Rest;

namespace Kook.Pipe;

internal static class PipeClientHelper
{
    public static async Task<Guid> SendTextAsync(KookPipeClient client, string text,
        IQuote? quote, ulong? ephemeralUserId, RequestOptions? options)
    {
        CreatePipeMessageParams args = new()
        {
            Content = text
        };
        CreateMessageResponse response = await client.ApiClient
            .CreatePipeMessageAsync(args, MessageType.KMarkdown, quote,ephemeralUserId, options)
            .ConfigureAwait(false);
        return response.MessageId;
    }

    public static async Task<Guid> SendTextAsync<T>(KookPipeClient client, T? parameters,
        IQuote? quote, ulong? ephemeralUserId, JsonSerializerOptions? jsonSerializerOptions, RequestOptions? options)
    {
        CreateMessageResponse response = await client.ApiClient
            .CreatePipeMessageAsync(parameters, jsonSerializerOptions, MessageType.KMarkdown, quote, ephemeralUserId, options)
            .ConfigureAwait(false);
        return response.MessageId;
    }

    public static async Task<Guid> SendCardsAsync(KookPipeClient client, IEnumerable<ICard> cards,
        IQuote? quote, ulong? ephemeralUserId, RequestOptions? options)
    {
        string json = MessageHelper.SerializeCards(cards);
        CreatePipeMessageParams args = new()
        {
            Content = json
        };
        CreateMessageResponse response = await client.ApiClient
            .CreatePipeMessageAsync(args, MessageType.Card, quote, ephemeralUserId, options)
            .ConfigureAwait(false);
        return response.MessageId;
    }

    public static async Task<Guid> SendCardsAsync<T>(KookPipeClient client, T? parameters,
        IQuote? quote, ulong? ephemeralUserId, JsonSerializerOptions? jsonSerializerOptions, RequestOptions? options)
    {
        CreateMessageResponse response = await client.ApiClient
            .CreatePipeMessageAsync(parameters, jsonSerializerOptions, MessageType.Card, quote, ephemeralUserId, options)
            .ConfigureAwait(false);
        return response.MessageId;
    }
}
