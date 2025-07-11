using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class GetThreadCategoriesResponse
{
    [JsonPropertyName("list")]
    public required ThreadCategory[] List { get; set; }
}
