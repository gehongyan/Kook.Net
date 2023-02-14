using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class ElementTypeConverter : JsonConverter<ElementType>
{
    public override ElementType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string type = reader.GetString();
        return type switch
        {
            "plain-text" => ElementType.PlainText,
            "kmarkdown" => ElementType.KMarkdown,
            "image" => ElementType.Image,
            "button" => ElementType.Button,
            "paragraph" => ElementType.Paragraph,
            _ => throw new ArgumentOutOfRangeException(nameof(CardType))
        };
    }

    public override void Write(Utf8JsonWriter writer, ElementType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            ElementType.PlainText => "plain-text",
            ElementType.KMarkdown => "kmarkdown",
            ElementType.Image => "image",
            ElementType.Button => "button",
            ElementType.Paragraph => "paragraph",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        });
    }
}
