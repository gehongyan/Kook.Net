using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class CardThemeConverter : JsonConverter<CardTheme>
{
    public override CardTheme Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? theme = reader.GetString();
        return theme switch
        {
            "primary" => CardTheme.Primary,
            "success" => CardTheme.Success,
            "warning" => CardTheme.Warning,
            "danger" => CardTheme.Danger,
            "info" => CardTheme.Info,
            "secondary" => CardTheme.Secondary,
            "none" => CardTheme.None,
            "invisible" => CardTheme.Invisible,
            _ => throw new ArgumentOutOfRangeException(nameof(CardTheme))
        };
    }

    public override void Write(Utf8JsonWriter writer, CardTheme value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            CardTheme.Primary => "primary",
            CardTheme.Success => "success",
            CardTheme.Warning => "warning",
            CardTheme.Danger => "danger",
            CardTheme.Info => "info",
            CardTheme.Secondary => "secondary",
            CardTheme.None => "none",
            CardTheme.Invisible => "invisible",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        });
}
