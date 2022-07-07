using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class EndGameActivityParams
{
    public EndGameActivityParams(ActivityType activityType)
    {
        ActivityType = activityType;
    }
    
    [JsonInclude]
    [JsonPropertyName("data_type")]
    public ActivityType ActivityType { get; private set; }
}