using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Voice;

internal class VoiceSocketResponseFrame
{
    [JsonPropertyName("response")]
    public bool Response { get; set; }
    
    [JsonPropertyName("id")]
    public uint Id { get; set; }
    
    [JsonPropertyName("ok")]
    public bool Okay { get; set; }
    
    [JsonPropertyName("data")]
    public object Payload { get; set; }
}