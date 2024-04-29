using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class SafeAttachmentConverter : JsonConverter<API.Attachment?>
{
    /// <inheritdoc />
    public override API.Attachment? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.StartArray)
        {
            reader.Skip();
            return null;
        }

        API.Attachment attachment = new()
        {
            Type = string.Empty,
            Url = string.Empty,
        };
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return attachment;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            string? propertyName = reader.GetString();
            reader.Read();
            switch (propertyName)
            {
                case "type":
                    attachment.Type = reader.GetString()
                        ?? throw new JsonException("Required property missing: type");
                    break;
                case "url":
                    attachment.Url = reader.GetString()
                        ?? throw new JsonException("Required property missing: url");
                    break;
                case "name":
                    attachment.Name = reader.GetString();
                    break;
                case "file_type":
                    attachment.FileType = reader.GetString();
                    break;
                case "size":
                    attachment.Size = reader.GetInt32();
                    break;
                case "duration":
                    attachment.Duration = reader.GetDouble();
                    break;
                case "width":
                    attachment.Width = reader.GetInt32();
                    break;
                case "height":
                    attachment.Height = reader.GetInt32();
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        throw new JsonException($"Unexpected end when reading {nameof(API.Attachment)}.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, API.Attachment? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteString("type", value.Type);
        writer.WriteString("url", value.Url);
        writer.WriteString("name", value.Name);
        writer.WriteString("file_type", value.FileType);
        if (value.Size.HasValue)
            writer.WriteNumber("size", value.Size.Value);
        if (value.Duration.HasValue)
            writer.WriteNumber("duration", value.Duration.Value);
        if (value.Width.HasValue)
            writer.WriteNumber("width", value.Width.Value);
        if (value.Height.HasValue)
            writer.WriteNumber("height", value.Height.Value);
        writer.WriteEndObject();
    }
}
