using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class NullableGuidConverter : JsonConverter<Guid?>
{
    /// <inheritdoc />
    public override bool HandleNull => true;

    public override Guid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;
        return Guid.TryParse(reader.GetString(), out Guid guid)
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
