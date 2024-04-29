using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class GetFriendStatesResponse
{
    [JsonPropertyName("request")]
    public required FriendState[] FriendRequests { get; set; }

    [JsonPropertyName("friend")]
    public required FriendState[] Friends { get; set; }

    [JsonPropertyName("blocked")]
    public required FriendState[] BlockedUsers { get; set; }
}
