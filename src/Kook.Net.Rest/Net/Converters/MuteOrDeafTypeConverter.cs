using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API.Rest;

namespace Kook.Net.Converters;

internal class MuteOrDeafTypeConverter : JsonConverter<MuteOrDeafType>
{
    /// <inheritdoc />
    public override MuteOrDeafType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String when reader.GetString() is { } stringValue:
                return (MuteOrDeafType)Enum.Parse(typeof(MuteOrDeafType), stringValue, true);
            case JsonTokenType.Number when reader.GetInt32() is { } intValue:
                return (MuteOrDeafType)intValue;
            default:
                throw new JsonException();
        }
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, MuteOrDeafType value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int)value);
    }
}
