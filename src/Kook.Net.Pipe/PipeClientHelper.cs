using System.Text.Json;
using Kook.API.Rest;

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

    public static async Task<Guid> SendMessageAsync(KookPipeClient client, string text, RequestOptions? options)
    {
        CreatePipeMessageParams args = new()
        {
            Content = text
        };
        CreateMessageResponse response = await client.ApiClient
            .CreatePipeMessageAsync(args, options).ConfigureAwait(false);
        return response.MessageId;
    }
}
