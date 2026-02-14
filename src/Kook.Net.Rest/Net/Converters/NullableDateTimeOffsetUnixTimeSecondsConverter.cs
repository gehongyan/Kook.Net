using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class NullableDateTimeOffsetUnixTimeSecondsConverter : JsonConverter<DateTimeOffset?>
{
    /// <inheritdoc />
    public override bool HandleNull => true;

    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;
        long tick = reader.GetInt64();
        if (tick == 0)
            return null;

        return DateTimeOffset.FromUnixTimeSeconds(tick).ToLocalTime();
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (!value.HasValue || value == DateTimeOffset.MinValue || value == DateTimeOffset.UnixEpoch)
            writer.WriteNullValue();
        else
            writer.WriteNumberValue(value.Value.ToUnixTimeSeconds());
    }
}
