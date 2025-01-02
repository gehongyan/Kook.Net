using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class GetFriendStatesResponse
{
    [JsonPropertyName("request")]
    public required FriendState[] FriendRequests { get; set; }

    [JsonPropertyName("all_request")]
    public required FriendState[] RelationRequests { get; set; }

    [JsonPropertyName("friend")]
    public required FriendState[] Friends { get; set; }

    [JsonPropertyName("blocked")]
    public required FriendState[] BlockedUsers { get; set; }

    [JsonPropertyName("relation")]
    public required FriendState[] Relations { get; set; }
}
