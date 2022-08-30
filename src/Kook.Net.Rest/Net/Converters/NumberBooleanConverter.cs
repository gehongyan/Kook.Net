using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

public class NumberBooleanConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.True:
                return true;
            case JsonTokenType.False:
                return false;
            case JsonTokenType.Number:
                return reader.TryGetInt32(out int value) && value == 1;
            default:
                throw new JsonException($"{nameof(NumberBooleanConverter)} expects boolean or number token, but got {reader.TokenType}");
        }
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }
}