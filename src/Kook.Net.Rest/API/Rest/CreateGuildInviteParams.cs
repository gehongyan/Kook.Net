using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateGuildInviteParams
{
    [JsonPropertyName("guild_id")]
    [JsonConverter(typeof(NullableUInt64Converter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("channel_id")]
    [JsonConverter(typeof(NullableUInt64Converter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? ChannelId { get; set; }

    [JsonPropertyName("duration")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InviteMaxAge? MaxAge { get; set; }

    [JsonPropertyName("setting_times")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InviteMaxUses? MaxUses { get; set; }
}
