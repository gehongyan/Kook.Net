using System.Text.Json.Serialization;
using KaiHeiLa.API.Converters;

namespace KaiHeiLa.API;

public class SocketHelloPayload
{
    [JsonPropertyName("code")]
    public int Code { get; set; }
    
    [JsonPropertyName("session_id")]
    [JsonConverter(typeof(GuidConverter))]
    public Guid SessionId { get; set; }
}