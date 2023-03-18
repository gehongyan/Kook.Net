using Kook.API.Gateway;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class GatewaySocketFrameTypeConverter : JsonConverter<GatewaySocketFrameType>
{
    public override GatewaySocketFrameType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        (GatewaySocketFrameType)reader.GetInt32();

    public override void Write(Utf8JsonWriter writer, GatewaySocketFrameType value, JsonSerializerOptions options) =>
        writer.WriteNumberValue((int)value);
}
