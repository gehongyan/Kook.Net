using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API.Rest;

namespace Kook.Net.Converters;

internal class ContentFilterTargetItemJsonConverter : JsonConverter<ContentFilterTargetItem>
{
    /// <inheritdoc />
    public override ContentFilterTargetItem? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.StartObject)
        {
            ContentFilterTargetItem item = new();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;
                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Expected PropertyName token");
                string propertyName = reader.GetString()!;
                reader.Read();
                switch (propertyName)
                {
                    case "id":
                        item.Id = reader.TokenType is JsonTokenType.Number ? reader.GetUInt64() : ulong.Parse(reader.GetString()!);
                        break;
                    case "name":
                        item.Name = reader.GetString();
                        break;
                    case "icon":
                        item.Icon = reader.GetString();
                        break;
                }
            }

            return item;
        }

        if (reader.TokenType is JsonTokenType.String)
        {
            return new ContentFilterTargetItem
            {
                Id = ulong.Parse(reader.GetString()!)
            };
        }

        return null;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ContentFilterTargetItem value, JsonSerializerOptions options)
    {
        if (!string.IsNullOrWhiteSpace(value.Name) || !string.IsNullOrWhiteSpace(value.Icon))
        {
            writer.WriteStartObject();
            writer.WriteString("id", value.Id.ToString());
            if (!string.IsNullOrWhiteSpace(value.Name))
                writer.WriteString("name", value.Name);
            if (!string.IsNullOrWhiteSpace(value.Icon))
                writer.WriteString("icon", value.Icon);
            writer.WriteEndObject();
        }
        else
            writer.WriteStringValue(value.Id.ToString());
    }
}
