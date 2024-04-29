using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class ElementBase : IElement
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(ElementTypeConverter))]
    public required ElementType Type { get; set; }
}
