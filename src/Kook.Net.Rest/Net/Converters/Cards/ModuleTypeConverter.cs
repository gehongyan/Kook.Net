using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class ModuleTypeConverter : JsonConverter<ModuleType>
{
    public override ModuleType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string type = reader.GetString();
        return type switch
        {
            "header" => ModuleType.Header,
            "section" => ModuleType.Section,
            "image-group" => ModuleType.ImageGroup,
            "container" => ModuleType.Container,
            "action-group" => ModuleType.ActionGroup,
            "context" => ModuleType.Context,
            "divider" => ModuleType.Divider,
            "file" => ModuleType.File,
            "audio" => ModuleType.Audio,
            "video" => ModuleType.Video,
            "countdown" => ModuleType.Countdown,
            "invite" => ModuleType.Invite,
            _ => throw new ArgumentOutOfRangeException(nameof(CardType))
        };
    }

    public override void Write(Utf8JsonWriter writer, ModuleType value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            ModuleType.Header => "header",
            ModuleType.Section => "section",
            ModuleType.ImageGroup => "image-group",
            ModuleType.Container => "container",
            ModuleType.ActionGroup => "action-group",
            ModuleType.Context => "context",
            ModuleType.Divider => "divider",
            ModuleType.File => "file",
            ModuleType.Audio => "audio",
            ModuleType.Video => "video",
            ModuleType.Countdown => "countdown",
            ModuleType.Invite => "invite",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        });
}
