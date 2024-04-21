using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class UnblockUserParams
{
    [JsonPropertyName("user_id")]
    public required ulong UserId { get; set; }
}
