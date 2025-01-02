using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class UnravelRelationParams
{
    [JsonPropertyName("user_id")]
    public required ulong UserId { get; set; }

    [JsonPropertyName("is_remove_friend")]
    public required bool RemoveFriend { get; set; }
}
