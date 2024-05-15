using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

/// <summary>
///
/// </summary>
public class ChannelTypeConverter : JsonConverter<ChannelType>
{
    /// <inheritdoc />
    public override ChannelType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number:
                return (ChannelType)reader.GetInt32();
            case JsonTokenType.String:
                return (ChannelType)Enum.Parse(typeof(ChannelType), reader.GetString() ?? string.Empty, true);
            default:
                throw new JsonException();
        }
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ChannelType value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int)value);
    }
}
