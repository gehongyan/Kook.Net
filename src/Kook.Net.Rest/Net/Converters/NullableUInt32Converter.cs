using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class NullableUInt32Converter : JsonConverter<uint?>
{
    public override uint? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                string value = reader.GetString();
                return !string.IsNullOrWhiteSpace(value) && uint.TryParse(value, out uint result)
                    ? result
                    : null;
            case JsonTokenType.Number:
                return reader.GetUInt32();
            default:
                throw new JsonException($"{nameof(NullableUInt32Converter)} expects string or number token, but got {reader.TokenType}");
        }
    }

    public override void Write(Utf8JsonWriter writer, uint? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.ToString());
    }
}
