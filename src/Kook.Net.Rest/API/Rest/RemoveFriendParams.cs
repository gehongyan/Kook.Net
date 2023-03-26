using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class RemoveFriendParams
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }
}
