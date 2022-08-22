using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Gateway;

internal class GatewayGroupMessageExtraData
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(MessageTypeConverter))]
    public MessageType Type { get; set; }
    
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("channel_name")]
    public string ChannelName { get; set; }

    [JsonPropertyName("mention")]
    public ulong[] Mention { get; set; }

    [JsonPropertyName("mention_all")]
    public bool MentionAll { get; set; }

    [JsonPropertyName("mention_roles")]
    public uint[] MentionRoles { get; set; }

    [JsonPropertyName("mention_here")]
    public bool MentionHere { get; set; }

    [JsonPropertyName("author")]
    public Rest.GuildMember Author { get; set; }

    [JsonPropertyName("quote")]
    public Quote Quote { get; set; }
    
    [JsonPropertyName("attachments")]
    public Attachment Attachment { get; set; }
    
    [JsonPropertyName("kmarkdown")]
    public KMarkdownInfo KMarkdownInfo { get; set; }
}