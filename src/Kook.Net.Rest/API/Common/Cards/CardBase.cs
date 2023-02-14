using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class CardBase : ICard
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(CardTypeConverter))]
    public CardType Type { get; set; }
}
