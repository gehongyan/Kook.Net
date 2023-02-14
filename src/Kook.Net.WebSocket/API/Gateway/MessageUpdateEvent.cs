using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class MessageUpdateEvent
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("mention")]
    public ulong[] Mention { get; set; }

    [JsonPropertyName("mention_all")]
    public bool MentionAll { get; set; }

    [JsonPropertyName("mention_here")]
    public bool MentionHere { get; set; }

    [JsonPropertyName("mention_roles")]
    public uint[] MentionRoles { get; set; }

    [JsonPropertyName("updated_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonPropertyName("msg_id")]
    public Guid MessageId { get; set; }
}
