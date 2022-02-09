using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class ParagraphStruct : ElementBase
{
    [JsonPropertyName("cols")]
    public int ColumnCount { get; set; }

    [JsonPropertyName("fields")]
    public ElementBase[] Fields { get; set; }
}