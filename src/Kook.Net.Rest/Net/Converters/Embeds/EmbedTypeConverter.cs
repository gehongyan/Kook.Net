using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class EmbedTypeConverter : JsonConverter<EmbedType>
{
    public override EmbedType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string type = reader.GetString();
        return type switch
        {
            "link" => EmbedType.Link,
            "image" => EmbedType.Image,
            "bili-video" => EmbedType.BilibiliVideo,
            _ => EmbedType.NotImplemented
        };
    }

    public override void Write(Utf8JsonWriter writer, EmbedType value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            EmbedType.Link => "link",
            EmbedType.Image => "image",
            EmbedType.BilibiliVideo => "bili-video",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        });
}
