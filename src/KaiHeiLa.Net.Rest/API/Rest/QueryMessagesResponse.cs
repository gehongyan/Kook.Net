using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class QueryMessagesResponse
{
    [JsonPropertyName("items")]
    public Message[] Items { get; set; }
}