using Kook.API;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Kook.Net.Rest;

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
            "link" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.LinkEmbed>()),
            "image" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.ImageEmbed>()),
            "bili-video" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.BilibiliVideoEmbed>()),
            "card" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.CardEmbed>()),
            _ => new API.NotImplementedEmbed(rawType, jsonNode)
        };
    }

    public override void Write(Utf8JsonWriter writer, EmbedBase value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case API.LinkEmbed { Type: EmbedType.Link } link:
                writer.WriteRawValue(JsonSerializer.Serialize(link, options.GetTypedTypeInfo<API.LinkEmbed>()));
                break;
            case API.ImageEmbed { Type: EmbedType.Image } image:
                writer.WriteRawValue(JsonSerializer.Serialize(image, options.GetTypedTypeInfo<API.ImageEmbed>()));
                break;
            case API.BilibiliVideoEmbed { Type: EmbedType.BilibiliVideo } bilibiliVideo:
                writer.WriteRawValue(JsonSerializer.Serialize(bilibiliVideo, options.GetTypedTypeInfo<API.BilibiliVideoEmbed>()));
                break;
            case API.CardEmbed { Type: EmbedType.Card } card:
                writer.WriteRawValue(JsonSerializer.Serialize(card, options.GetTypedTypeInfo<API.CardEmbed>()));
                break;
            default:
                writer.WriteRawValue((value as API.NotImplementedEmbed)!.RawJsonNode.ToString());
                break;
        }
    }
}
