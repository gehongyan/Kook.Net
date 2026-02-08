using Kook.API;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Kook.Rest;

namespace Kook.Net.Converters;

internal class ModuleConverter : JsonConverter<ModuleBase>
{
    public static readonly ModuleConverter Instance = new();

    public override ModuleBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonNode? jsonNode = JsonNode.Parse(ref reader);
        return jsonNode?["type"]?.GetValue<string>() switch
        {
            "header" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.HeaderModule>()),
            "section" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.SectionModule>()),
            "image-group" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.ImageGroupModule>()),
            "container" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.ContainerModule>()),
            "action-group" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.ActionGroupModule>()),
            "context" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.ContextModule>()),
            "divider" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.DividerModule>()),
            "file" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.FileModule>()),
            "audio" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.AudioModule>()),
            "video" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.VideoModule>()),
            "countdown" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.CountdownModule>()),
            "invite" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.InviteModule>()),
            _ => throw new ArgumentOutOfRangeException(nameof(CardType))
        };
    }

    public override void Write(Utf8JsonWriter writer, ModuleBase value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case API.HeaderModule { Type: ModuleType.Header } header:
                writer.WriteRawValue(JsonSerializer.Serialize(header, options.GetTypedTypeInfo<API.HeaderModule>()));
                break;
            case API.SectionModule { Type: ModuleType.Section } section:
                writer.WriteRawValue(JsonSerializer.Serialize(section, options.GetTypedTypeInfo<API.SectionModule>()));
                break;
            case API.ImageGroupModule { Type: ModuleType.ImageGroup } imageGroup:
                writer.WriteRawValue(JsonSerializer.Serialize(imageGroup, options.GetTypedTypeInfo<API.ImageGroupModule>()));
                break;
            case API.ContainerModule { Type: ModuleType.Container } container:
                writer.WriteRawValue(JsonSerializer.Serialize(container, options.GetTypedTypeInfo<API.ContainerModule>()));
                break;
            case API.ActionGroupModule { Type: ModuleType.ActionGroup } actionGroup:
                writer.WriteRawValue(JsonSerializer.Serialize(actionGroup, options.GetTypedTypeInfo<API.ActionGroupModule>()));
                break;
            case API.ContextModule { Type: ModuleType.Context } context:
                writer.WriteRawValue(JsonSerializer.Serialize(context, options.GetTypedTypeInfo<API.ContextModule>()));
                break;
            case API.DividerModule { Type: ModuleType.Divider } divider:
                writer.WriteRawValue(JsonSerializer.Serialize(divider, options.GetTypedTypeInfo<API.DividerModule>()));
                break;
            case API.FileModule { Type: ModuleType.File } file:
                writer.WriteRawValue(JsonSerializer.Serialize(file, options.GetTypedTypeInfo<API.FileModule>()));
                break;
            case API.AudioModule { Type: ModuleType.Audio } audio:
                writer.WriteRawValue(JsonSerializer.Serialize(audio, options.GetTypedTypeInfo<API.AudioModule>()));
                break;
            case API.VideoModule { Type: ModuleType.Video } video:
                writer.WriteRawValue(JsonSerializer.Serialize(video, options.GetTypedTypeInfo<API.VideoModule>()));
                break;
            case API.CountdownModule { Type: ModuleType.Countdown } countdown:
                writer.WriteRawValue(JsonSerializer.Serialize(countdown, options.GetTypedTypeInfo<API.CountdownModule>()));
                break;
            case API.InviteModule { Type: ModuleType.Invite } invite:
                writer.WriteRawValue(JsonSerializer.Serialize(invite, options.GetTypedTypeInfo<API.InviteModule>()));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(ModuleType));
        }
    }
}
