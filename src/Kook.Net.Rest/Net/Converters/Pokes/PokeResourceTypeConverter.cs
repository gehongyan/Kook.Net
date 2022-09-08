using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class PokeResourceTypeConverter : JsonConverter<PokeResourceType>
{
    public override PokeResourceType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string type = reader.GetString();
        return type switch
        {
            "ImageAnimation" => PokeResourceType.ImageAnimation,
            _ => PokeResourceType.NotImplemented
        };
    }

    public override void Write(Utf8JsonWriter writer, PokeResourceType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            PokeResourceType.ImageAnimation => "ImageAnimation",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        });
    }
}