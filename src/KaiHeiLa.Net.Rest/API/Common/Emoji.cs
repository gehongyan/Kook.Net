using System.Text.Json.Serialization;

namespace KaiHeiLa.API;

internal class Emoji
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
}