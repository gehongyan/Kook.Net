using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class NullableUInt64Converter : JsonConverter<ulong?>
{
    /// <inheritdoc />
    public override bool HandleNull => true;

    public override ulong? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;
        string? value = reader.GetString();
        return !string.IsNullOrWhiteSpace(value) && ulong.TryParse(value, out ulong result)
            ? result
            : null;
    }

    public override void Write(Utf8JsonWriter writer, ulong? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.ToString());
    }
}
