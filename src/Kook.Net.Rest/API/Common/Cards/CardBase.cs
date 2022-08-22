using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class CardBase : ICard
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(CardTypeConverter))]
    public CardType Type { get; set; }
}