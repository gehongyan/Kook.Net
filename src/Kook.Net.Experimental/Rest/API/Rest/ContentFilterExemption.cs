using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Rest;

internal class ContentFilterExemption
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(ContentFilterExemptionTypeJsonConverter))]
    public ContentFilterExemptionType Type { get; set; }

    [JsonPropertyName("id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public ulong Id { get; set; }
}
