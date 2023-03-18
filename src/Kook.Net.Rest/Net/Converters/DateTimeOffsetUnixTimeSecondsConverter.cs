using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class DateTimeOffsetUnixTimeSecondsConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64());

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        if (value == DateTimeOffset.MinValue || value == DateTimeOffset.UnixEpoch)
#else
        if (value == DateTimeOffset.MinValue || value == new DateTimeOffset(621355968000000000L, TimeSpan.Zero))
#endif
            writer.WriteNullValue();
        else
            writer.WriteNumberValue(value.ToUnixTimeSeconds());
    }
}
