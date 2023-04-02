using System.Text.Json.Serialization;
using Kook.Rest;

namespace Kook.API.Rest;

internal class ValidateCardsParams
{
    [JsonPropertyName("content")]
    public string Content { get; set; }

    public static implicit operator ValidateCardsParams(string content) => new() {Content = content};

    public static ValidateCardsParams FromCards(IEnumerable<Kook.ICard> cards) => MessageHelper.SerializeCards(cards);
}
