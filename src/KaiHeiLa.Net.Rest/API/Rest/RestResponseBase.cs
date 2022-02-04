using System.Text.Json.Serialization;

namespace KaiHeiLa.API;

internal class RestResponseBase
{
    [JsonPropertyName("code")]
    public KaiHeiLaErrorCode Code { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; }
    
    [JsonPropertyName("data")]
    public object Data { get; set; }
}