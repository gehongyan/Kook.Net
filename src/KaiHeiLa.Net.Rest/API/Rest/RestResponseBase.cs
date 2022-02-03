using System.Text.Json.Serialization;

namespace KaiHeiLa.API;

internal class RestResponseBase<TData>
{
    [JsonPropertyName("code")]
    public int Code { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; }
    
    [JsonPropertyName("data")]
    public TData Data { get; set; }
}