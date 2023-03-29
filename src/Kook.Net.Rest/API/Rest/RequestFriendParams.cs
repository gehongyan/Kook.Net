using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class RequestFriendParams
{
    [JsonPropertyName("user_code")]
    public string FullQualification { get; set; }

    [JsonPropertyName("from")]
    public RequestFriendSource Source { get; set; }

    [JsonPropertyName("guild_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ulong GuildId { get; set; }
}
