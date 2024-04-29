using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class ChatCodeConverter : JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        Guid.Parse(reader.GetString() ?? throw new InvalidCastException("Cannot convert null to Guid"));

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString("N"));
}
