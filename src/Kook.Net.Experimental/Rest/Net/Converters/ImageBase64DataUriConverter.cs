using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Kook.Net.Converters;

internal class ImageBase64DataUriConverter : JsonConverter<Image?>
{
    public override bool HandleNull => true;

    public override Image? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string raw = reader.GetString();
        if (string.IsNullOrWhiteSpace(raw)) return null;

        Match match = Regex.Match(raw, @"^data:image/(\w+?-)?(?<type>\w+?);base64,(?<data>.+)$");
        string type = match.Groups["type"].Value;
        string data = match.Groups["data"].Value;
        byte[] binaryData = Convert.FromBase64String(data);
        MemoryStream stream = new(binaryData);
        return new Image(stream, type);
    }

    public override void Write(Utf8JsonWriter writer, Image? value, JsonSerializerOptions options)
    {
        if (!value.HasValue)
        {
            writer.WriteStringValue(string.Empty);
            return;
        }

        byte[] bytes;
        using (MemoryStream memoryStream = new())
        {
            value.Value.Stream.CopyTo(memoryStream);
            bytes = memoryStream.ToArray();
        }

        string base64 = Convert.ToBase64String(bytes);
        string extension = !string.IsNullOrWhiteSpace(value?.FileExtension)
            ? value.Value.FileExtension
            : "png";
        writer.WriteStringValue($"data:image/{extension};base64,{base64}");
    }
}
