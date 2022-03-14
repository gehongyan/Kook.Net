using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

public class BeginGameActivityParams
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonInclude]
    [JsonPropertyName("data_type")]
    public int DataType { get; private set; } = 1;
    
    public static implicit operator BeginGameActivityParams(int id) => new() {Id = id};
}