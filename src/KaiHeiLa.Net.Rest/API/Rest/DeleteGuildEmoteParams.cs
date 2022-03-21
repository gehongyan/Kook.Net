using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class DeleteGuildEmoteParams
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    public static implicit operator DeleteGuildEmoteParams(string id) => new() {Id = id};
}