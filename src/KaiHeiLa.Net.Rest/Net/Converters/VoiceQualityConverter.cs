using System.Text.Json;
using System.Text.Json.Serialization;
using KaiHeiLa.API.Rest;

namespace KaiHeiLa.Net.Converters;

internal class VoiceQualityConverter : JsonConverter<VoiceQuality>
{
    public override VoiceQuality Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return (VoiceQuality) reader.GetInt32();
    }

    public override void Write(Utf8JsonWriter writer, VoiceQuality value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int) value);
    }
}