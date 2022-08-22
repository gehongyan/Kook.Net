using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class ElementBase : IElement
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(ElementTypeConverter))]
    public ElementType Type { get; set; }
}