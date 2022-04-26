using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class EndGameActivityParams
{
    [JsonInclude]
    [JsonPropertyName("data_type")]
    public int DataType { get; private set; } = 1;
}