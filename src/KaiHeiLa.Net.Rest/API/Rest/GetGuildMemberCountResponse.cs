using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class GetGuildMemberCountResponse
{
    [JsonPropertyName("user_count")]
    public int UserCount { get; set; }
    [JsonPropertyName("online_count")]
    public int OnlineCount { get; set; }
    [JsonPropertyName("offline_count")]
    public int OfflineCount { get; set; }
}