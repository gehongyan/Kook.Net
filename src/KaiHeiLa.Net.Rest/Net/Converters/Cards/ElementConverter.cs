using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using KaiHeiLa.API;

namespace KaiHeiLa.Net.Converters;

internal class ElementConverter : JsonConverter<ElementBase>
{
    public override ElementBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonNode jsonNode = JsonNode.Parse(ref reader);
        return jsonNode["type"].GetValue<string>() switch
        {
            "plain-text" => JsonSerializer.Deserialize<API.PlainTextElement>(jsonNode.ToJsonString(), options),
            "kmarkdown" => JsonSerializer.Deserialize<API.KMarkdownElement>(jsonNode.ToJsonString(), options),
            "image" => JsonSerializer.Deserialize<API.ImageElement>(jsonNode.ToJsonString(), options),
            "button" => JsonSerializer.Deserialize<API.ButtonElement>(jsonNode.ToJsonString(), options),
            "paragraph" => JsonSerializer.Deserialize<API.ParagraphStruct>(jsonNode.ToJsonString(), options),
            _ => throw new ArgumentOutOfRangeException(nameof(ElementType))
        };
        ;
    }

    public override void Write(Utf8JsonWriter writer, ElementBase value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}