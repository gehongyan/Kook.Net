using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class SectionAccessoryModeConverter : JsonConverter<SectionAccessoryMode>
{
    public override SectionAccessoryMode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string mode = reader.GetString();
        return mode switch
        {
            "left" => SectionAccessoryMode.Left,
            "right" => SectionAccessoryMode.Right,
            _ => SectionAccessoryMode.Unspecified
        };
    }

    public override void Write(Utf8JsonWriter writer, SectionAccessoryMode value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            SectionAccessoryMode.Left => "left",
            SectionAccessoryMode.Right => "right",
            SectionAccessoryMode.Unspecified => null,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        });
    }
}
