using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Kook.Audio.WebRtc.Signaling;

/// <summary>
///     protoo 4.x 的 JSON 帧编解码。帧格式：
///     <c>{"request":true,"id":1,"method":"m","data":{}}</c>、
///     <c>{"response":true,"id":1,"ok":true,"data":{}}</c>、
///     <c>{"response":true,"id":1,"ok":false,"errorCode":404,"errorReason":"..."}</c>、
///     <c>{"notification":true,"method":"m","data":{}}</c>。
/// </summary>
internal static class ProtooMessageCodec
{
    private static readonly JsonWriterOptions WriterOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    ///     解码一帧文本。无法识别（非 JSON、非对象、缺少必需字段）时返回 <c>null</c>，
    ///     例如心跳 <c>ping</c> 的应答可能不是 protoo 帧。
    /// </summary>
    public static ProtooMessage? Decode(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(text);
        }
        catch (JsonException)
        {
            return null;
        }

        using (document)
        {
            JsonElement root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
                return null;

            if (IsTrue(root, "request"))
            {
                return TryGetInt64(root, "id", out long id) && TryGetString(root, "method", out string? method)
                    ? new ProtooRequest(id, method, GetData(root))
                    : null;
            }

            if (IsTrue(root, "response"))
            {
                if (!TryGetInt64(root, "id", out long id))
                    return null;
                bool ok = IsTrue(root, "ok");
                int? errorCode = TryGetInt64(root, "errorCode", out long code) ? (int)code : null;
                TryGetString(root, "errorReason", out string? errorReason);
                return new ProtooResponse(id, ok, GetData(root), errorCode, errorReason);
            }

            if (IsTrue(root, "notification"))
            {
                return TryGetString(root, "method", out string? method)
                    ? new ProtooNotification(method, GetData(root))
                    : null;
            }

            return null;
        }
    }

    public static string EncodeRequest(long id, string method, object? data) => Write(writer =>
    {
        writer.WriteBoolean("request", true);
        writer.WriteNumber("id", id);
        writer.WriteString("method", method);
        WriteData(writer, data);
    });

    public static string EncodeNotification(string method, object? data) => Write(writer =>
    {
        writer.WriteBoolean("notification", true);
        writer.WriteString("method", method);
        WriteData(writer, data);
    });

    public static string EncodeResponse(long id, object? data) => Write(writer =>
    {
        writer.WriteBoolean("response", true);
        writer.WriteNumber("id", id);
        writer.WriteBoolean("ok", true);
        WriteData(writer, data);
    });

    public static string EncodeErrorResponse(long id, int errorCode, string errorReason) => Write(writer =>
    {
        writer.WriteBoolean("response", true);
        writer.WriteNumber("id", id);
        writer.WriteBoolean("ok", false);
        writer.WriteNumber("errorCode", errorCode);
        writer.WriteString("errorReason", errorReason);
    });

    private static string Write(Action<Utf8JsonWriter> body)
    {
        using MemoryStream stream = new();
        using (Utf8JsonWriter writer = new(stream, WriterOptions))
        {
            writer.WriteStartObject();
            body(writer);
            writer.WriteEndObject();
        }
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static void WriteData(Utf8JsonWriter writer, object? data)
    {
        if (data is null)
            return;
        writer.WritePropertyName("data");
        if (data is JsonElement element)
            element.WriteTo(writer);
        else
            JsonSerializer.Serialize(writer, data, data.GetType(), ProtooJson.Options);
    }

    private static bool IsTrue(JsonElement root, string name) =>
        root.TryGetProperty(name, out JsonElement value) && value.ValueKind == JsonValueKind.True;

    private static bool TryGetString(JsonElement root, string name, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? value)
    {
        value = null;
        if (!root.TryGetProperty(name, out JsonElement element) || element.ValueKind != JsonValueKind.String)
            return false;
        value = element.GetString();
        return value is not null;
    }

    private static bool TryGetInt64(JsonElement root, string name, out long value)
    {
        value = 0;
        if (!root.TryGetProperty(name, out JsonElement element))
            return false;
        return element.ValueKind switch
        {
            JsonValueKind.Number => element.TryGetInt64(out value),
            JsonValueKind.String => long.TryParse(element.GetString(), out value),
            _ => false
        };
    }

    private static JsonElement? GetData(JsonElement root) =>
        root.TryGetProperty("data", out JsonElement data) && data.ValueKind is not (JsonValueKind.Null or JsonValueKind.Undefined)
            ? data.Clone()
            : null;
}
