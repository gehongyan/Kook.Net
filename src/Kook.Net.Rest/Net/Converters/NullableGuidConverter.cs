using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class NullableGuidConverter : JsonConverter<Guid?>
{
    public override Guid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Guid.TryParse(reader.GetString() ?? string.Empty, out Guid guid)
            ? guid
            : Guid.Empty;
    }

    public override void Write(Utf8JsonWriter writer, Guid? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else if (value == Guid.Empty)
            writer.WriteStringValue(string.Empty);
        else
            writer.WriteStringValue(value.ToString());
    }
}
