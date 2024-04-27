using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class GatewayGroupMessageExtraData
{
    [JsonPropertyName("type")]
    public MessageType Type { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("channel_name")]
    public required string ChannelName { get; set; }

    [JsonPropertyName("author")]
    public required Rest.GuildMember Author { get; set; }

    [JsonPropertyName("mention")]
    public required ulong[] MentionedUsers { get; set; }

    [JsonPropertyName("mention_all")]
    public bool MentionedAll { get; set; }

    [JsonPropertyName("mention_roles")]
    public required uint[] MentionedRoles { get; set; }

    [JsonPropertyName("mention_here")]
    public bool MentionedHere { get; set; }

    [JsonPropertyName("nav_channels")]
    public required ulong[] MentionedChannels { get; set; }

    [JsonPropertyName("kmarkdown")]
    public required KMarkdownInfo KMarkdownInfo { get; set; }

    [JsonPropertyName("quote")]
    [JsonConverter(typeof(QuoteConverter))]
    public Quote? Quote { get; set; }

    [JsonPropertyName("attachments")]
    public Attachment? Attachment { get; set; }
}
