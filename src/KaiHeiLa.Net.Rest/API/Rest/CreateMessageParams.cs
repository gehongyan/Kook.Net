using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class CreateMessageParams
{
    [JsonPropertyName("type")] public MessageType Type { get; set; }

    [JsonPropertyName("target_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("content")] public string Content { get; set; }

    [JsonPropertyName("quote")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? QuotedMessageId { get; set; }

    [JsonPropertyName("nonce")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Nonce { get; set; }

    [JsonPropertyName("temp_target_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong? EphemeralUserId { get; set; }
    
    public CreateMessageParams(MessageType messageType, ulong channelId, string content)
    {
        Type = messageType;
        ChannelId = channelId;
        Content = content;
    }
}