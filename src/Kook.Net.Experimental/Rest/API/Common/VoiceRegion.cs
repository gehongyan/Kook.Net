using System.Text.Json.Serialization;

namespace Kook.API;

internal class VoiceRegion
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("crowding")]
    public int Crowding { get; set; }

    [JsonPropertyName("level")]
    public BoostLevel MinimumBoostLevel { get; set; }
}
