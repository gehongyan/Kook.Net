using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class BeginActivityParams
{
    [JsonPropertyName("data_type")]
    public required ActivityType ActivityType { get; set; }

    // Game
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    // Music
    [JsonPropertyName("software")]
    [JsonConverter(typeof(MusicProviderConverter))]
    public MusicProvider MusicProvider { get; set; }

    [JsonPropertyName("singer")]
    public string? Signer { get; set; }

    [JsonPropertyName("music_name")]
    public string? MusicName { get; set; }
}
