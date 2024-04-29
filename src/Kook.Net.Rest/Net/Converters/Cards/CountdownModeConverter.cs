using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class CountdownModeConverter : JsonConverter<CountdownMode>
{
    public override CountdownMode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? mode = reader.GetString();
        return mode switch
        {
            "day" => CountdownMode.Day,
            "hour" => CountdownMode.Hour,
            "second" => CountdownMode.Second,
            _ => throw new ArgumentOutOfRangeException(nameof(CountdownMode))
        };
    }

    public override void Write(Utf8JsonWriter writer, CountdownMode value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            CountdownMode.Day => "day",
            CountdownMode.Hour => "hour",
            CountdownMode.Second => "second",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        });
}
