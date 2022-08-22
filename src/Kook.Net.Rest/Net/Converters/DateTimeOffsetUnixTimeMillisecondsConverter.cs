using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class DateTimeOffsetUnixTimeMillisecondsConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64());
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        if (value == DateTimeOffset.MinValue || value == DateTimeOffset.FromUnixTimeMilliseconds(0))
            writer.WriteNullValue();
        else
            writer.WriteNumberValue(value.ToUnixTimeMilliseconds());
    }
}