using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class QueryThreadsResponse
{
    [JsonPropertyName("items")]
    public required ExtendedThread[] Items { get; set; }
}
