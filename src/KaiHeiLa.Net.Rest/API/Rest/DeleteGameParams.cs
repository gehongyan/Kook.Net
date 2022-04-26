using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class DeleteGameParams
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    public static implicit operator DeleteGameParams(int id) => new() {Id = id};
}