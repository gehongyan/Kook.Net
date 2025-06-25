using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class ColorMap
{
    [JsonPropertyName("color_list")]
    public uint[]? ColorList { get; set; }
}

