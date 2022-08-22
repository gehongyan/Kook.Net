using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Rest;

internal class BeginActivityParams
{
    public BeginActivityParams(ActivityType activityType)
    {
        ActivityType = activityType;
    }

    [JsonInclude]
    [JsonPropertyName("data_type")]
    public ActivityType ActivityType { get; private set; }
    
    // Game
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    // Music
    [JsonPropertyName("software")]
    [JsonConverter(typeof(MusicProviderConverter))]
    public MusicProvider MusicProvider { get; set; }
    
    [JsonPropertyName("singer")]
    public string Signer { get; set; }
    
    [JsonPropertyName("music_name")]
    public string MusicName { get; set; }
}