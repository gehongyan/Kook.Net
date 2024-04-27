using Kook.API;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class ModuleConverter : JsonConverter<ModuleBase>
{
    public static readonly ModuleConverter Instance = new();

    public override ModuleBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonNode? jsonNode = JsonNode.Parse(ref reader);
        return jsonNode?["type"]?.GetValue<string>() switch
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
    }

    public override void Write(Utf8JsonWriter writer, ModuleBase value, JsonSerializerOptions options)
    {
        switch (value.Type)
        {
            case ModuleType.Header:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.HeaderModule, options));
                break;
            case ModuleType.Section:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.SectionModule, options));
                break;
            case ModuleType.ImageGroup:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.ImageGroupModule, options));
                break;
            case ModuleType.Container:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.ContainerModule, options));
                break;
            case ModuleType.ActionGroup:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.ActionGroupModule, options));
                break;
            case ModuleType.Context:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.ContextModule, options));
                break;
            case ModuleType.Divider:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.DividerModule, options));
                break;
            case ModuleType.File:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.FileModule, options));
                break;
            case ModuleType.Audio:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.AudioModule, options));
                break;
            case ModuleType.Video:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.VideoModule, options));
                break;
            case ModuleType.Countdown:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.CountdownModule, options));
                break;
            case ModuleType.Invite:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.InviteModule, options));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(ModuleType));
        }
    }
}
