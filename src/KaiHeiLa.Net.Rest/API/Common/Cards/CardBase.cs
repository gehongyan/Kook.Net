using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class CardBase : ICard
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(CardTypeConverter))]
    public CardType Type { get; set; }
}