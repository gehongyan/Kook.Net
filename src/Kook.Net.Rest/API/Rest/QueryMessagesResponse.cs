using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class QueryMessagesResponse
{
    [JsonPropertyName("items")]
    public Message[] Items { get; set; }
}
