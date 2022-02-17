using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class ModifyGuildEmoteParams
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
}