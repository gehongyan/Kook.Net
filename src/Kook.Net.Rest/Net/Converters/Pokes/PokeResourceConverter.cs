using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Kook.API;

namespace Kook.Net.Converters;

internal class PokeResourceConverter : JsonConverter<PokeResourceBase>
{
    public override PokeResourceBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonNode jsonNode = JsonNode.Parse(ref reader);
        string rawType = jsonNode["type"].GetValue<string>();
        return rawType switch
        {
            "ImageAnimation" => JsonSerializer.Deserialize<API.ImageAnimationPokeResource>(jsonNode.ToJsonString(), options),
            _ => new API.NotImplementedPokeResource(rawType, jsonNode)
        };
    }

    public override void Write(Utf8JsonWriter writer, PokeResourceBase value, JsonSerializerOptions options)
    {
        switch (value.Type)
        {
            case PokeResourceType.ImageAnimation:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.ImageAnimationPokeResource, options));
                break;
            default:
                writer.WriteRawValue((value as API.NotImplementedPokeResource)!.RawJsonNode.ToString());
                break;
        }
    }
}