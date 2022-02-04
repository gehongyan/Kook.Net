using System.Text.Json;
using System.Text.Json.Serialization;

namespace KaiHeiLa.Net.Converters;

internal class NotifyTypeConverter : JsonConverter<NotifyType>
{
    public override NotifyType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return (NotifyType) reader.GetInt32();
    }

    public override void Write(Utf8JsonWriter writer, NotifyType value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int) value);
    }
}