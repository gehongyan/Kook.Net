using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class NullableTimeSpanConverter : JsonConverter<TimeSpan?>
{
    public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ulong tick = reader.GetUInt64();
        if (tick == 0) return null;
        return TimeSpan.FromSeconds(tick);
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNumberValue(0);
        else writer.WriteNumberValue(value.Value.TotalSeconds);
    }
}