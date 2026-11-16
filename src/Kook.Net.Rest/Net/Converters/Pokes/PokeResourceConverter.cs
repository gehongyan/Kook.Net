using Kook.API;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Kook.Rest;

namespace Kook.Net.Converters;

internal class PokeResourceConverter : JsonConverter<PokeResourceBase>
{
    public override PokeResourceBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonNode? jsonNode = JsonNode.Parse(ref reader);
        if (jsonNode == null) return null;
        string? rawType = jsonNode["type"]?.GetValue<string>();
        if (rawType == null) return null;
        return rawType switch
        {
            "ImageAnimation" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.ImageAnimationPokeResource>()),
            _ => new API.NotImplementedPokeResource(rawType, jsonNode) { Type = PokeResourceType.NotImplemented }
        };
    }

    public override void Write(Utf8JsonWriter writer, PokeResourceBase value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case API.ImageAnimationPokeResource { Type : PokeResourceType.ImageAnimation } imageAnimation:
                writer.WriteRawValue(JsonSerializer.Serialize(imageAnimation, options.GetTypedTypeInfo<API.ImageAnimationPokeResource>()));
                break;
            default:
                writer.WriteRawValue((value as API.NotImplementedPokeResource)!.RawJsonNode.ToString());
                break;
        }
    }
}
