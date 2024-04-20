using Kook.API;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class EmbedConverter : JsonConverter<EmbedBase>
{
    public override EmbedBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonNode? jsonNode = JsonNode.Parse(ref reader);
        if (jsonNode == null) return null;
        string? rawType = jsonNode["type"]?.GetValue<string>();
        if (rawType == null) return null;
        return rawType switch
        {
            "link" => JsonSerializer.Deserialize<API.LinkEmbed>(jsonNode.ToJsonString(), options),
            "image" => JsonSerializer.Deserialize<API.ImageEmbed>(jsonNode.ToJsonString(), options),
            "bili-video" => JsonSerializer.Deserialize<API.BilibiliVideoEmbed>(jsonNode.ToJsonString(), options),
            _ => new API.NotImplementedEmbed(rawType, jsonNode)
        };
    }

    public override void Write(Utf8JsonWriter writer, EmbedBase value, JsonSerializerOptions options)
    {
        switch (value.Type)
        {
            case EmbedType.Link:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.LinkEmbed, options));
                break;
            case EmbedType.Image:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.ImageEmbed, options));
                break;
            case EmbedType.BilibiliVideo:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.BilibiliVideoEmbed, options));
                break;
            default:
                writer.WriteRawValue((value as API.NotImplementedEmbed)!.RawJsonNode.ToString());
                break;
        }
    }
}
