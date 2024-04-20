using Kook.API;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class CardConverter : JsonConverter<CardBase>
{
    public override CardBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonNode? jsonNode = JsonNode.Parse(ref reader);
        switch (jsonNode?["type"]?.GetValue<string>())
        {
            case "card":
                return JsonSerializer.Deserialize<API.Card>(jsonNode.ToJsonString(), options);
            default:
                throw new ArgumentOutOfRangeException(nameof(CardType));
        }
    }

    public override void Write(Utf8JsonWriter writer, CardBase value, JsonSerializerOptions options)
    {
        switch (value.Type)
        {
            case CardType.Card:
                writer.WriteRawValue(JsonSerializer.Serialize(value as API.Card, options));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(CardType));
        }
    }
}
