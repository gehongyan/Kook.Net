using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class ButtonClickEventTypeConverter : JsonConverter<ButtonClickEventType>
{
    public override ButtonClickEventType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string type = reader.GetString();
        return type switch
        {
            "" => ButtonClickEventType.None,
            "link" => ButtonClickEventType.Link,
            "return-val" => ButtonClickEventType.ReturnValue,
            _ => throw new ArgumentOutOfRangeException(nameof(ButtonClickEventType))
        };
    }

    public override void Write(Utf8JsonWriter writer, ButtonClickEventType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            ButtonClickEventType.None => "",
            ButtonClickEventType.Link => "link",
            ButtonClickEventType.ReturnValue => "return-val",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        });
    }
}