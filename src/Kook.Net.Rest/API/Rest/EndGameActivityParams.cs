using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class EndGameActivityParams
{
    [JsonPropertyName("data_type")]
    public required ActivityType ActivityType { get; set; }
}
