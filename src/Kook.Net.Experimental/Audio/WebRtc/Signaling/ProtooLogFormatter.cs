using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Kook.Audio.WebRtc.Signaling;

/// <summary>
///     将信令帧中的 JSON 数据渲染为单行文本用于调试日志，并隐藏凭据类字段的值。
/// </summary>
internal static class ProtooLogFormatter
{
    private const int MaxLength = 600;

    private static readonly Regex SensitiveKey =
        new("token|password|credential|secret|authorization|username|cookie|^sign$", RegexOptions.IgnoreCase);

    private static readonly JsonWriterOptions WriterOptions = new()
    {
        Indented = false,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    internal static string RenderData(JsonElement? data)
    {
        if (data is not { } element)
            return "(none)";

        using MemoryStream stream = new();
        using (Utf8JsonWriter writer = new(stream, WriterOptions))
            Write(writer, element, string.Empty);
        string text = Encoding.UTF8.GetString(stream.ToArray());
        return text.Length <= MaxLength ? text : $"{text.Substring(0, MaxLength)}...(+{text.Length - MaxLength})";
    }

    private static void Write(Utf8JsonWriter writer, JsonElement element, string key)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                foreach (JsonProperty property in element.EnumerateObject())
                {
                    writer.WritePropertyName(property.Name);
                    Write(writer, property.Value, property.Name);
                }
                writer.WriteEndObject();
                break;
            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (JsonElement item in element.EnumerateArray())
                    Write(writer, item, key);
                writer.WriteEndArray();
                break;
            case JsonValueKind.String when SensitiveKey.IsMatch(key):
                writer.WriteStringValue($"<redacted len={element.GetString()?.Length ?? 0}>");
                break;
            default:
                element.WriteTo(writer);
                break;
        }
    }
}
