using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class GetFriendStatesResponse
{
    [JsonPropertyName("request")]
    public FriendState[] FriendRequests { get; set; }

    [JsonPropertyName("friend")]
    public FriendState[] Friends { get; set; }

    [JsonPropertyName("blocked")]
    public FriendState[] BlockedUsers { get; set; }
}
