using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Rest;

[JsonConverter(typeof(ContentFilterTargetItemJsonConverter))]
internal class ContentFilterTargetItem
{
    [JsonPropertyName("id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    [JsonPropertyName("icon")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Icon { get; set; }
}
