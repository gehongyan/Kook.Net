using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Gateway;

internal class GatewayGroupMessageExtraData
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(MessageTypeConverter))]
    public MessageType Type { get; set; }
    
    [JsonPropertyName("guild_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong GuildId { get; set; }

    [JsonPropertyName("channel_name")]
    public string ChannelName { get; set; }

    [JsonPropertyName("mention")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong[] Mention { get; set; }

    [JsonPropertyName("mention_all")]
    public bool MentionAll { get; set; }

    [JsonPropertyName("mention_roles")]
    public uint[] MentionRoles { get; set; }

    [JsonPropertyName("mention_here")]
    public bool MentionHere { get; set; }

    [JsonPropertyName("author")]
    public User Author { get; set; }

    [JsonPropertyName("quote")]
    public Quote Quote { get; set; }
    
    [JsonPropertyName("attachments")]
    public Attachment Attachment { get; set; }
}