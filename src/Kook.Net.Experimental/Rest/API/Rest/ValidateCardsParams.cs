using System.Text.Json.Serialization;
using Kook.Net.Converters;
using Kook.Rest;

namespace Kook.API.Rest;

internal class ValidateCardsParams
{
    [JsonPropertyName("content")]
    public required string Content { get; set; }

    public static implicit operator ValidateCardsParams(string content) => new() { Content = content };

    public static ValidateCardsParams FromCards(IEnumerable<Kook.ICard> cards) => MessageHelper.SerializeCards(cards);
}
