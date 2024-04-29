using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class QuoteConverter : JsonConverter<API.Quote>
{
    /// <inheritdoc />
    public override API.Quote? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType switch
        {
            JsonTokenType.Null or JsonTokenType.String => null,
            JsonTokenType.StartObject => JsonSerializer.Deserialize<API.Quote>(ref reader, options),
            _ => throw new JsonException(
                $"{nameof(NumberBooleanConverter)} expects boolean, string or number token, but got {reader.TokenType}")
        };

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, API.Quote value, JsonSerializerOptions options) => throw new NotImplementedException();
}
