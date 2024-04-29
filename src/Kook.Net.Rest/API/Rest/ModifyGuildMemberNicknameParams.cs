using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class ModifyGuildMemberNicknameParams
{
    [JsonPropertyName("guild_id")]
    public required ulong GuildId { get; set; }

    [JsonPropertyName("nickname")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Nickname { get; set; }

    [JsonPropertyName("user_id")]
    [JsonConverter(typeof(NullableUInt64Converter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? UserId { get; set; }
}
