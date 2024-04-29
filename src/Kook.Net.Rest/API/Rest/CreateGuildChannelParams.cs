using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateGuildChannelParams
{
    [JsonPropertyName("guild_id")]
    public required ulong GuildId { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("parent_id")]
    [JsonConverter(typeof(NullableUInt64Converter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? CategoryId { get; set; }

    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ChannelType? Type { get; set; }

    [JsonPropertyName("limit_amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? LimitAmount { get; set; }

    [JsonPropertyName("voice_quality")]
    [JsonConverter(typeof(NullableVoiceQualityConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VoiceQuality? VoiceQuality { get; set; }

    [JsonPropertyName("is_category")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? IsCategory { get; set; }
}
