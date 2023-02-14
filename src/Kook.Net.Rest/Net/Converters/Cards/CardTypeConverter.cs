using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class CardTypeConverter : JsonConverter<CardType>
{
    public override CardType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string type = reader.GetString();
        return type switch
        {
            "card" => CardType.Card,
            _ => throw new ArgumentOutOfRangeException(nameof(CardType))
        };
    }

    public override void Write(Utf8JsonWriter writer, CardType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            CardType.Card => "card",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        });
    }
}
