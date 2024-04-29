using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class CardBase : ICard
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(CardTypeConverter))]
    public required CardType Type { get; set; }
}
