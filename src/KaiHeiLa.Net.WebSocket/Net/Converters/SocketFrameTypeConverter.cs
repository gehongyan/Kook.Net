using System.Text.Json;
using System.Text.Json.Serialization;
using KaiHeiLa.API;

namespace KaiHeiLa.Net.Converters;

internal class SocketFrameTypeConverter : JsonConverter<SocketFrameType>
{
    public override SocketFrameType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return (SocketFrameType) reader.GetInt32();
    }

    public override void Write(Utf8JsonWriter writer, SocketFrameType value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int) value);
    }
}