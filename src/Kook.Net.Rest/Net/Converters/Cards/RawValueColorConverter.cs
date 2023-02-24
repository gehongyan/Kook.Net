using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class RawValueColorConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        uint rawValue = reader.GetUInt32();
        return new Color(rawValue);
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.RawValue);
    }
}
