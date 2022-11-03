using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API.Voice;

namespace Kook.Net.Converters;

internal class VoiceSocketFrameTypeConverter : JsonConverter<VoiceSocketFrameType>
{
    public override VoiceSocketFrameType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Enum.TryParse(reader.GetString(), true, out VoiceSocketFrameType value)
            ? value
            : throw new ArgumentOutOfRangeException(nameof(VoiceSocketFrameType));
    }

    public override void Write(Utf8JsonWriter writer, VoiceSocketFrameType value, JsonSerializerOptions options)
    {
        string method = value.ToString();
        method = method.Length > 1
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            ? method[..1].ToLower() + method[1..]
#else
            ? method.Substring(0, 1).ToLower() + method.Substring(1)
#endif
            : method.ToLower();
        writer.WriteStringValue(method);
    }
}