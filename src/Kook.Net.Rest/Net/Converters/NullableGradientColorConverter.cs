using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class NullableGradientColorConverter : JsonConverter<GradientColor?>
{
    /// <inheritdoc />
    public override GradientColor? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException($"{nameof(NullableGradientColorConverter)} expects property name token, but got {reader.TokenType}");
            string propertyName = reader.GetString();
            if (propertyName != "color_list")
                throw new JsonException($"{nameof(NullableGradientColorConverter)} expects property name 'color_list', but got {propertyName}");
            reader.Read();
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException($"{nameof(NullableGradientColorConverter)} expects start array token, but got {reader.TokenType}");
            reader.Read();
            if (reader.TokenType != JsonTokenType.Number)
                throw new JsonException($"{nameof(NullableGradientColorConverter)} expects number token, but got {reader.TokenType}");
            var left = reader.GetUInt32();
            reader.Read();
            if (reader.TokenType != JsonTokenType.Number)
                throw new JsonException($"{nameof(NullableGradientColorConverter)} expects number token, but got {reader.TokenType}");
            var right = reader.GetUInt32();
            reader.Read();
            if (reader.TokenType != JsonTokenType.EndArray)
                throw new JsonException($"{nameof(NullableGradientColorConverter)} expects end array token, but got {reader.TokenType}");
            reader.Read();
            if (reader.TokenType != JsonTokenType.EndObject)
                throw new JsonException($"{nameof(NullableGradientColorConverter)} expects end object token, but got {reader.TokenType}");
            return new GradientColor(new Color(left), new Color(right));
        }

        if (reader.TokenType == JsonTokenType.StartArray)
        {
            reader.Read();
            if (reader.TokenType != JsonTokenType.EndArray)
                throw new JsonException($"{nameof(NullableGradientColorConverter)} expects end array token, but got {reader.TokenType}");
            return null;
        }

        throw new JsonException(
            $"{nameof(NullableGradientColorConverter)} expects start object or start array token, but got {reader.TokenType}");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, GradientColor? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("color_list");
            writer.WriteStartArray();
            writer.WriteNumberValue(value.Value.Left);
            writer.WriteNumberValue(value.Value.Right);
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
        else
        {
            writer.WriteStartArray();
            writer.WriteEndArray();
        }
    }
}
