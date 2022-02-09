using System.Text.Json;
using System.Text.Json.Serialization;

namespace KaiHeiLa.Net.Converters;

public class ColorConverter : JsonConverter<Color?>
{
    public override Color? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string hex = reader.GetString()?.TrimStart('#');
        if (string.IsNullOrWhiteSpace(hex) || hex.Length < 6)
            return null;
        return new Color(uint.Parse(hex, System.Globalization.NumberStyles.HexNumber));
    }

    public override void Write(Utf8JsonWriter writer, Color? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteStringValue(string.Empty);
        writer.WriteStringValue($"#{value.Value.RawValue:X6}");
    }
}