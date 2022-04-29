using System.Text.Json;
using System.Text.Json.Serialization;

namespace KaiHeiLa.Net.Converters;

internal class SectionAccessoryModeConverter : JsonConverter<SectionAccessoryMode>
{
    public override SectionAccessoryMode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string mode = reader.GetString();
        return mode switch
        {
            "left" => SectionAccessoryMode.Left,
            "right" => SectionAccessoryMode.Right,
            "" => SectionAccessoryMode.Unspecified,
            _ => throw new ArgumentOutOfRangeException(nameof(SectionAccessoryMode))
        };
    }

    public override void Write(Utf8JsonWriter writer, SectionAccessoryMode value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            SectionAccessoryMode.Left => "left",
            SectionAccessoryMode.Right => "right",
            SectionAccessoryMode.Unspecified => "",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        });
    }
}