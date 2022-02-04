using System.Text.Json;
using System.Text.Json.Serialization;

namespace KaiHeiLa.Net.Converters;

internal class NullableUInt64Converter : JsonConverter<ulong?>
{
    public override ulong? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string value = reader.GetString();
        return !string.IsNullOrWhiteSpace(value) && ulong.TryParse(value, out ulong result) 
            ? result 
            : null;
    }

    public override void Write(Utf8JsonWriter writer, ulong? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value is null
            ? string.Empty
            : value.ToString());
    }
}