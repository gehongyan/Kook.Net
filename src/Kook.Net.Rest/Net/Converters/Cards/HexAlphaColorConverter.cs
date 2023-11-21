using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class HexAlphaColorConverter : JsonConverter<AlphaColor>
{
    public override AlphaColor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string hex = reader.GetString()?.TrimStart('#');
        if (string.IsNullOrWhiteSpace(hex) || hex.Length < 8)
            return AlphaColor.Default;

        return new AlphaColor(uint.Parse(hex, System.Globalization.NumberStyles.HexNumber));
    }

    public override void Write(Utf8JsonWriter writer, AlphaColor value, JsonSerializerOptions options) =>
        writer.WriteStringValue($"#{value.RawValue:X8}");
}
