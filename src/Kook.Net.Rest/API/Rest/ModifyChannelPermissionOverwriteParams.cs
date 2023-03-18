using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class ModifyChannelPermissionOverwriteParams
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(PermissionOverwriteTargetTypeConverter))]
    public PermissionOverwriteTargetType TargetType { get; set; }

    [JsonPropertyName("value")]
    public ulong TargetId { get; set; }

    [JsonPropertyName("allow")]
    [JsonConverter(typeof(NullableUInt64Converter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? Allow { get; set; }

    [JsonPropertyName("deny")]
    [JsonConverter(typeof(NullableUInt64Converter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? Deny { get; set; }

    public ModifyChannelPermissionOverwriteParams(ulong channelId, PermissionOverwriteTargetType targetType, ulong targetId, ulong? allow,
        ulong? deny)
    {
        ChannelId = channelId;
        TargetType = targetType;
        TargetId = targetId;
        Allow = allow;
        Deny = deny;
    }
}
