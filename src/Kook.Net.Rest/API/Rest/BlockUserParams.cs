using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class BlockUserParams
{
    [JsonPropertyName("user_id")]
    public required ulong UserId { get; set; }
}
