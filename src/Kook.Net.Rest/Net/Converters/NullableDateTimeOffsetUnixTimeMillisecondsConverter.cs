using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class NullableDateTimeOffsetUnixTimeMillisecondsConverter : JsonConverter<DateTimeOffset?>
{
    /// <inheritdoc />
    public override bool HandleNull => true;

    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;
        return DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64());
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        if (!value.HasValue || value == DateTimeOffset.MinValue || value == DateTimeOffset.UnixEpoch)
#else
        if (!value.HasValue || value == DateTimeOffset.MinValue || value == new DateTimeOffset(621355968000000000L, TimeSpan.Zero))
#endif
            writer.WriteNullValue();
        else
            writer.WriteNumberValue(value.Value.ToUnixTimeMilliseconds());
    }
}
