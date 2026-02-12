using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API;
using Kook.Rest;

namespace Kook.Net.Converters;

internal class NullableChannelConverter : JsonConverter<API.Channel?>
{
    /// <inheritdoc />
    public override Channel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.StartObject:
                return JsonSerializer.Deserialize(ref reader, options.GetTypedTypeInfo<Channel>());
            case JsonTokenType.Null or JsonTokenType.StartArray:
                reader.Skip();
                return null;
            default:
                throw new JsonException();
        }
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Channel? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        JsonSerializer.Serialize(writer, value, options.GetTypedTypeInfo<Channel>());
    }
}
