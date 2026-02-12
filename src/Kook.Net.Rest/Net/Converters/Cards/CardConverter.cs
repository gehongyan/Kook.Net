using Kook.API;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Kook.Rest;

namespace Kook.Net.Converters;

internal class CardConverter : JsonConverter<CardBase>
{
    public static readonly CardConverter Instance = new();

    public override CardBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonNode? jsonNode = JsonNode.Parse(ref reader);
        switch (jsonNode?["type"]?.GetValue<string>())
        {
            case "card":
                return JsonSerializer.Deserialize(jsonNode.ToJsonString(), options.GetTypedTypeInfo<API.Card>());
            default:
                throw new ArgumentOutOfRangeException(nameof(CardType));
        }
    }

    public override void Write(Utf8JsonWriter writer, CardBase value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case API.Card { Type: CardType.Card } card:
                writer.WriteRawValue(JsonSerializer.Serialize(card, options.GetTypedTypeInfo<API.Card>()));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(CardType));
        }
    }
}
