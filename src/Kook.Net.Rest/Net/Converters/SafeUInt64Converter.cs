using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class SafeUInt64Converter : JsonConverter<ulong>
{
    /// <inheritdoc />
    public override bool HandleNull => true;

    public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return 0;
        string? value = reader.GetString();
        return !string.IsNullOrWhiteSpace(value) && ulong.TryParse(value, out ulong result)
            ? result
            : 0;
    }

    public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
