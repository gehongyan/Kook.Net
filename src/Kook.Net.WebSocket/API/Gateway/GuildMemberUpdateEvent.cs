using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Gateway;

internal class GuildMemberUpdateEvent
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    [JsonPropertyName("identify_num")]
    public required string IdentifyNumber { get; set; }

    [JsonPropertyName("online")]
    public bool Online { get; set; }

    [JsonPropertyName("bot")]
    public bool? Bot { get; set; }

    [JsonPropertyName("status")]
    public int? Status { get; set; }

    [JsonPropertyName("avatar")]
    public required string Avatar { get; set; }

    [JsonPropertyName("vip_avatar")]
    public string? BuffAvatar { get; set; }

    [JsonPropertyName("mobile_verified")]
    public bool MobileVerified { get; set; }

    [JsonPropertyName("roles")]
    public uint[]? Roles { get; set; }

    [JsonPropertyName("boost_start_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset? BoostStartAt { get; set; }
}
