using Kook.API.Voice;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class VoiceSocketFrameTypeConverter : JsonConverter<VoiceSocketFrameType>
{
    public override VoiceSocketFrameType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        new(reader.GetString() ?? string.Empty);

    public override void Write(Utf8JsonWriter writer, VoiceSocketFrameType value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}
