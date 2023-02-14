using Kook.Net.Converters;
using System.Text.Json.Serialization;

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
    public ulong[] MentionedUsers { get; set; }

    [JsonPropertyName("mention_all")]
    public bool MentionedAll { get; set; }

    [JsonPropertyName("mention_roles")]
    public uint[] MentionedRoles { get; set; }

    [JsonPropertyName("mention_here")]
    public bool MentionedHere { get; set; }

    [JsonPropertyName("nav_channels")]
    public ulong[] MentionedChannels { get; set; }

    [JsonPropertyName("author")]
    public Rest.GuildMember Author { get; set; }

    [JsonPropertyName("quote")]
    public Quote Quote { get; set; }

    [JsonPropertyName("attachments")]
    public Attachment Attachment { get; set; }

    [JsonPropertyName("kmarkdown")]
    public KMarkdownInfo KMarkdownInfo { get; set; }
}
