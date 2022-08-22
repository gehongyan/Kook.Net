using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class CountdownModule : ModuleBase
{
    [JsonPropertyName("endTime")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset EndTime { get; set; }
    
    [JsonPropertyName("startTime")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset? StartTime { get; set; }
    
    [JsonPropertyName("mode")]
    [JsonConverter(typeof(CountdownModeConverter))]
    public CountdownMode Mode { get; set; }
}