using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class QueryUserChatMessagesResponse
{
    [JsonPropertyName("items")]
    public DirectMessage[] Items { get; set; }
}