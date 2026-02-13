using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Rest;

internal class ContentFilterHandler
{
    [JsonPropertyName("type")]
    public ContentFilterHandlerType Type { get; set; }

    [JsonPropertyName("enable")]
    public bool Enabled { get; set; }

    [JsonPropertyName("custom_error_msg")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CustomErrorMessage { get; set; }

    [JsonPropertyName("alert_channel_id")]
    [JsonConverter(typeof(NullableUInt64Converter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? AlertChannelId { get; set; }

    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }
}
