using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class NullableDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
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

        return DateTimeOffset.FromUnixTimeMilliseconds(tick);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNumberValue(0);
        else
            writer.WriteNumberValue(value.Value.ToUnixTimeMilliseconds());
    }
}
