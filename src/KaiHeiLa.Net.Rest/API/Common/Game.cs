using System.Text.Json.Serialization;

namespace KaiHeiLa.API;

internal class Game
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("type")]
    public ActivityType Type { get; set; }
    
    [JsonPropertyName("options")]
    public string Options { get; set; }
    
    [JsonPropertyName("kmhook_admin")]
    public bool KmHookAdmin { get; set; }
    
    [JsonPropertyName("process_name")]
    public string[] ProcessNames { get; set; }
    
    [JsonPropertyName("product_name")]
    public string[] ProductNames { get; set; }
    
    [JsonPropertyName("icon")]
    public string Icon { get; set; }
}