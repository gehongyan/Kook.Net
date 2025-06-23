using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class ModifyMessageTemplateParams
{
    [JsonPropertyName("id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public required ulong Id { get; set; }

    [JsonPropertyName("title")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Title { get; set; }

    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TemplateType? Type { get; set; }

    [JsonPropertyName("test_channel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TestChannel { get; set; }

    [JsonPropertyName("msgtype")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TemplateMessageType? MessageType { set; get; }

    [JsonPropertyName("content")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Content { get; set; }

    [JsonPropertyName("status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TestData { get; set; }
}
