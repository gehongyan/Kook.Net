using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class ModifyChannelPermissionOverwriteParams
{
    [JsonPropertyName("channel_id")]
    public required ulong ChannelId { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(TypeIdPermissionOverwriteTargetTypeConverter))]
    public PermissionOverwriteTarget TargetType { get; set; }

    [JsonPropertyName("value")]
    public required ulong TargetId { get; set; }

    [JsonPropertyName("allow")]
    [JsonConverter(typeof(NullableUInt64Converter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? Allow { get; set; }

    [JsonPropertyName("deny")]
    [JsonConverter(typeof(NullableUInt64Converter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? Deny { get; set; }
}
