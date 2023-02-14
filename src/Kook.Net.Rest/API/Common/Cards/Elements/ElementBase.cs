using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class ElementBase : IElement
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(ElementTypeConverter))]
    public ElementType Type { get; set; }
}
