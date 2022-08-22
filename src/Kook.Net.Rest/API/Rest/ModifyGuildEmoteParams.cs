using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class ModifyGuildEmoteParams
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
}