using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API.Gateway;

namespace Kook.Net.Converters;

internal class GatewaySocketFrameTypeConverter : JsonConverter<GatewaySocketFrameType>
{
    public override GatewaySocketFrameType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return (GatewaySocketFrameType) reader.GetInt32();
    }

    public override void Write(Utf8JsonWriter writer, GatewaySocketFrameType value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int) value);
    }
}