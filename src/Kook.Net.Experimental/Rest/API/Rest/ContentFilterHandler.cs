using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class ContentFilterHandler
{
    [JsonPropertyName("type")]
    public ContentFilterHandlerType Type { get; set; }

    [JsonPropertyName("enable")]
    public bool Enabled { get; set; }

    [JsonPropertyName("custom_error_msg")]
    public string? CustomErrorMessage { get; set; }

    [JsonPropertyName("alert_channel_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public ulong? AlertChannelId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
