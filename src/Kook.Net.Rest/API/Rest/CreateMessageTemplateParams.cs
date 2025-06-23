using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateMessageTemplateParams
{
    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("type")]
    public TemplateType Type { get; set; }

    [JsonPropertyName("test_channel")]
    public string? TestChannel { get; set; }

    [JsonPropertyName("msgtype")]
    public TemplateMessageType MessageType { set; get; }

    [JsonPropertyName("content")]
    public required string Content { get; set; }

    [JsonPropertyName("test_data")]
    public string? TestData { get; set; }
}
