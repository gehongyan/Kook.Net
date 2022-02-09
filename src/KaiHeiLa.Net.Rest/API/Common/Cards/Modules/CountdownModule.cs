using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class CountdownModule : ModuleBase
{
    [JsonPropertyName("endTime")]
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset EndTime { get; set; }
    
    [JsonPropertyName("startTime")]
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset? StartTime { get; set; }
    
    [JsonPropertyName("mode")]
    [JsonConverter(typeof(CountdownModeConverter))]
    public CountdownMode Mode { get; set; }
}