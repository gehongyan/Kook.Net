using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class RequestFriendParams
{
    [JsonPropertyName("user_code")]
    public required string FullQualification { get; set; }

    [JsonPropertyName("from")]
    public required RequestFriendSource Source { get; set; }

    [JsonPropertyName("guild_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? GuildId { get; set; }
}
