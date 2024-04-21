using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class DeleteGameParams
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }

    public static implicit operator DeleteGameParams(int id) => new() { Id = id };
}
