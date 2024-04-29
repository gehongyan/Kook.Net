using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class DeleteGuildEmoteParams
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    public static implicit operator DeleteGuildEmoteParams(string id) => new() { Id = id };
}
