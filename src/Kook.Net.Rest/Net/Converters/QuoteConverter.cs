using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.Rest;

namespace Kook.Net.Converters;

/// <remarks>
///     不允许标记在 API.Quote 上，因为这会导致反序列化无限递归进 StartObject 分支。
/// </remarks>
internal class QuoteConverter : JsonConverter<API.Quote>
{
    /// <inheritdoc />
    public override API.Quote? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType switch
        {
            JsonTokenType.Null or JsonTokenType.String => null,
            // 此转换器不会直接标记在 API.Quote 上，而是标记在属性上，因此，直接对 reader 反序列化为 API.Quote 的操作不会使用此转换器。
            JsonTokenType.StartObject => JsonSerializer.Deserialize(ref reader, options.GetTypedTypeInfo<API.Quote>()),
            _ => throw new JsonException(
                $"{nameof(QuoteConverter)} expects boolean, string or number token, but got {reader.TokenType}")
        };

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, API.Quote value, JsonSerializerOptions options) => throw new NotImplementedException();
}
