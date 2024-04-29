using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class ImageSizeConverter : JsonConverter<ImageSize>
{
    public override ImageSize Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? size = reader.GetString();
        return size switch
        {
            "sm" => ImageSize.Small,
            "lg" => ImageSize.Large,
            _ => throw new ArgumentOutOfRangeException(nameof(CardSize))
        };
    }

    public override void Write(Utf8JsonWriter writer, ImageSize value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            ImageSize.Small => "sm",
            ImageSize.Large => "lg",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        });
}
