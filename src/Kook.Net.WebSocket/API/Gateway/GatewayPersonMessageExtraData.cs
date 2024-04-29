using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class GatewayPersonMessageExtraData
{
    [JsonPropertyName("type")]
    public MessageType Type { get; set; }

    [JsonPropertyName("code")]
    [JsonConverter(typeof(ChatCodeConverter))]
    public Guid Code { get; set; }

    [JsonPropertyName("author")]
    public required User Author { get; set; }

    [JsonPropertyName("mention")]
    public ulong[]? MentionedUsers { get; set; }

    [JsonPropertyName("quote")]
    [JsonConverter(typeof(QuoteConverter))]
    public Quote? Quote { get; set; }

    [JsonPropertyName("attachments")]
    [JsonConverter(typeof(SafeAttachmentConverter))]
    public Attachment? Attachment { get; set; }

    [JsonPropertyName("kmarkdown")]
    public required KMarkdownInfo KMarkdownInfo { get; set; }
}
