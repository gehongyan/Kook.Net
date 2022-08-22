using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class CardSizeConverter : JsonConverter<CardSize>
{
    public override CardSize Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string size = reader.GetString();
        return size switch
        {
            "sm" => CardSize.Small,
            "lg" => CardSize.Large,
            _ => throw new ArgumentOutOfRangeException(nameof(CardSize))
        };
    }

    public override void Write(Utf8JsonWriter writer, CardSize value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            CardSize.Small => "sm",
            CardSize.Large => "lg",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        });
    }
}