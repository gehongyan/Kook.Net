using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class ContentFilterExemptionTypeJsonConverter : JsonConverter<ContentFilterExemptionType>
{
    /// <inheritdoc />
    public override ContentFilterExemptionType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.String)
            return (ContentFilterExemptionType) Enum.Parse(typeof(ContentFilterExemptionType), reader.GetString()!);
        if (reader.TokenType is JsonTokenType.Number)
            return (ContentFilterExemptionType) reader.GetInt32();
        throw new JsonException($"Unexpected token parsing ContentFilterExemptionType. Token: {reader.TokenType}.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ContentFilterExemptionType value, JsonSerializerOptions options) =>
        writer.WriteStringValue(((int)value).ToString());
}
