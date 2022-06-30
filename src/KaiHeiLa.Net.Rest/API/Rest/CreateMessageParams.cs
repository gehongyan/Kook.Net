using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Rest;

internal class CreateMessageParams
{
    [JsonPropertyName("type")] public MessageType Type { get; set; }

    [JsonPropertyName("target_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("content")] public string Content { get; set; }

    [JsonPropertyName("quote")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? QuotedMessageId { get; set; }

    [JsonPropertyName("nonce")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Nonce { get; set; }

    [JsonPropertyName("temp_target_id")]
    [JsonConverter(typeof(NullableUInt64Converter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? EphemeralUserId { get; set; }
    
    public CreateMessageParams(MessageType messageType, ulong channelId, string content)
    {
        Type = messageType;
        ChannelId = channelId;
        Content = content;
    }
}