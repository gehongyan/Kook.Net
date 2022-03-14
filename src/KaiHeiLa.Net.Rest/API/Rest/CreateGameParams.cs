using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class CreateGameParams
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("process_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ProcessName { get; set; }
    
    [JsonPropertyName("icon")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Icon { get; set; }
}