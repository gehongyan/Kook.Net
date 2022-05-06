using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Gateway;

internal class GatewayPersonMessageExtraData
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(MessageTypeConverter))]
    public MessageType Type { get; set; }
    
    [JsonPropertyName("code")]
    [JsonConverter(typeof(ChatCodeConverter))]
    public Guid Code { get; set; }

    [JsonPropertyName("author")]
    public User Author { get; set; }
    
    [JsonPropertyName("nonce")]
    public string Nonce { get; set; }

    [JsonPropertyName("last_msg_content")]
    public string LastMessageContent { get; set; }
    
    [JsonPropertyName("quote")]
    public Quote Quote { get; set; }
    
    [JsonPropertyName("attachments")]
    public Attachment Attachment { get; set; }
    
    [JsonPropertyName("kmarkdown")]
    public KMarkdownInfo KMarkdownInfo { get; set; }
}