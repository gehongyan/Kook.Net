using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class NullableVoiceQualityConverter : JsonConverter<VoiceQuality?>
{
    /// <inheritdoc />
    public override bool HandleNull => true;

    public override VoiceQuality? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                string? str = reader.GetString();
                return int.TryParse(str, out int result)
                    ? (VoiceQuality?)result
                    : null;
            case JsonTokenType.Number:
                return reader.TryGetInt32(out int value)
                    ? (VoiceQuality?)value
                    : null;
            case JsonTokenType.Null:
                return null;
            default:
                throw new JsonException($"VoiceQuality expects string or number token, but got {reader.TokenType}");
        }
    }

    public override void Write(Utf8JsonWriter writer, VoiceQuality? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteNumberValue((int)value);
    }
}
