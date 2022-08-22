using System.Text.Json;
using System.Text.Json.Serialization;

namespace KaiHeiLa.Net.Converters;

internal class ButtonThemeConverter : JsonConverter<ButtonTheme>
{
    public override ButtonTheme Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string theme = reader.GetString();
        return theme switch
        {
            "primary" => ButtonTheme.Primary,
            "success" => ButtonTheme.Success,
            "warning" => ButtonTheme.Warning,
            "info" => ButtonTheme.Info,
            "danger" => ButtonTheme.Danger,
            "secondary" => ButtonTheme.Secondary,
            _ => throw new ArgumentOutOfRangeException(nameof(ButtonTheme))
        };
    }

    public override void Write(Utf8JsonWriter writer, ButtonTheme value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            ButtonTheme.Primary => "primary",
            ButtonTheme.Success => "success",
            ButtonTheme.Warning => "warning",
            ButtonTheme.Info => "info",
            ButtonTheme.Danger => "danger",
            ButtonTheme.Secondary => "secondary",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        });
    }
}