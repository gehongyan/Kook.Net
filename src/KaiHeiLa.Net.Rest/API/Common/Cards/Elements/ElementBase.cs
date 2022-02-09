using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class ElementBase : IElement
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(ElementTypeConverter))]
    public ElementType Type { get; set; }
}