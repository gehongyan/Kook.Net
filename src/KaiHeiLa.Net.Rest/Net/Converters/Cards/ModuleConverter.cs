using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using KaiHeiLa.API;

namespace KaiHeiLa.Net.Converters;

internal class ModuleConverter : JsonConverter<ModuleBase>
{
    public override ModuleBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonNode jsonNode = JsonNode.Parse(ref reader);
        return jsonNode["type"].GetValue<string>() switch
        {
            "header" => JsonSerializer.Deserialize<API.HeaderModule>(jsonNode.ToJsonString(), options),
            "section" => JsonSerializer.Deserialize<API.SectionModule>(jsonNode.ToJsonString(), options),
            "image-group" => JsonSerializer.Deserialize<API.ImageGroupModule>(jsonNode.ToJsonString(), options),
            "container" => JsonSerializer.Deserialize<API.ContainerModule>(jsonNode.ToJsonString(), options),
            "action-group" => JsonSerializer.Deserialize<API.ActionGroupModule>(jsonNode.ToJsonString(), options),
            "context" => JsonSerializer.Deserialize<API.ContextModule>(jsonNode.ToJsonString(), options),
            "divider" => JsonSerializer.Deserialize<API.DividerModule>(jsonNode.ToJsonString(), options),
            "file" => JsonSerializer.Deserialize<API.FileModule>(jsonNode.ToJsonString(), options),
            "audio" => JsonSerializer.Deserialize<API.AudioModule>(jsonNode.ToJsonString(), options),
            "video" => JsonSerializer.Deserialize<API.VideoModule>(jsonNode.ToJsonString(), options),
            "countdown" => JsonSerializer.Deserialize<API.CountdownModule>(jsonNode.ToJsonString(), options),
            "invite" => JsonSerializer.Deserialize<API.InviteModule>(jsonNode.ToJsonString(), options),
            _ => throw new ArgumentOutOfRangeException(nameof(CardType))
        };
        ;
    }

    public override void Write(Utf8JsonWriter writer, ModuleBase value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}