using Kook.API;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class ElementConverter : JsonConverter<ElementBase>
{
    public static readonly ElementConverter Instance = new();

    public override ElementBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonNode? jsonNode = JsonNode.Parse(ref reader);
        return jsonNode?["type"]?.GetValue<string>() switch
        {
            "plain-text" => JsonSerializer.Deserialize<API.PlainTextElement>(jsonNode.ToJsonString(), options),
            "kmarkdown" => JsonSerializer.Deserialize<API.KMarkdownElement>(jsonNode.ToJsonString(), options),
            "image" => JsonSerializer.Deserialize<API.ImageElement>(jsonNode.ToJsonString(), options),
            "button" => JsonSerializer.Deserialize<API.ButtonElement>(jsonNode.ToJsonString(), options),
            "paragraph" => JsonSerializer.Deserialize<API.ParagraphStruct>(jsonNode.ToJsonString(), options),
            _ => throw new ArgumentOutOfRangeException(nameof(ElementType))
        };
    }

    public override void Write(Utf8JsonWriter writer, ElementBase value, JsonSerializerOptions options)
    {
        switch (value.Type)
        {
            case ElementType.PlainText:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.PlainTextElement, options));
                break;
            case ElementType.KMarkdown:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.KMarkdownElement, options));
                break;
            case ElementType.Image:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.ImageElement, options));
                break;
            case ElementType.Button:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.ButtonElement, options));
                break;
            case ElementType.Paragraph:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.ParagraphStruct, options));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(ElementType));
        }
    }
}
