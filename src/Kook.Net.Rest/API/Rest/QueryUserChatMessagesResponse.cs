using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class QueryUserChatMessagesResponse
{
    [JsonPropertyName("items")]
    public DirectMessage[] Items { get; set; }
}
