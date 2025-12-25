using Kook.API;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Kook.Net.Rest;

namespace Kook.Net.Converters;

internal class ElementConverter : JsonConverter<ElementBase>
{
    public static readonly ElementConverter Instance = new();

    public override ElementBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonNode? jsonNode = JsonNode.Parse(ref reader);
        return jsonNode?["type"]?.GetValue<string>() switch
        {
            "plain-text" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.PlainTextElement>()),
            "kmarkdown" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.KMarkdownElement>()),
            "image" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.ImageElement>()),
            "button" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.ButtonElement>()),
            "paragraph" => JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.ParagraphStruct>()),
            _ => throw new ArgumentOutOfRangeException(nameof(ElementType))
        };
    }

    public override void Write(Utf8JsonWriter writer, ElementBase value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case API.PlainTextElement { Type: ElementType.PlainText } plaintext:
                writer.WriteRawValue(JsonSerializer.Serialize(plaintext, options.GetTypedTypeInfo<API.PlainTextElement>()));
                break;
            case API.KMarkdownElement { Type: ElementType.KMarkdown } kMarkdown:
                writer.WriteRawValue(JsonSerializer.Serialize(kMarkdown, options.GetTypedTypeInfo<API.KMarkdownElement>()));
                break;
            case API.ImageElement { Type: ElementType.Image } image:
                writer.WriteRawValue(JsonSerializer.Serialize(image, options.GetTypedTypeInfo<API.ImageElement>()));
                break;
            case API.ButtonElement { Type: ElementType.Button } button:
                writer.WriteRawValue(JsonSerializer.Serialize(button, options.GetTypedTypeInfo<API.ButtonElement>()));
                break;
            case API.ParagraphStruct { Type: ElementType.Paragraph } paragraph:
                writer.WriteRawValue(JsonSerializer.Serialize(paragraph, options.GetTypedTypeInfo<API.ParagraphStruct>()));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(ElementType));
        }
    }
}
