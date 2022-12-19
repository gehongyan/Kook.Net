using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Rest;

internal class CreateGuildParams
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("icon")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonConverter(typeof(ImageBase64DataUriConverter))]
    public Image? Icon { get; set; }

    [JsonPropertyName("region")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string RegionId { get; set; }

    [JsonPropertyName("template_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? TemplateId { get; set; }
}
