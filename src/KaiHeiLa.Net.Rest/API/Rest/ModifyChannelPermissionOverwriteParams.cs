using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Rest;

internal class ModifyChannelPermissionOverwriteParams
{
    [JsonPropertyName("channel_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(PermissionOverwriteTargetTypeConverter))]
    public PermissionOverwriteTargetType TargetType { get; set; }

    [JsonPropertyName("value")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public ulong TargetId { get; set; }

    [JsonPropertyName("allow")] 
    [JsonConverter(typeof(NullableUInt64Converter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? Allow { get; set; }
    
    [JsonPropertyName("deny")] 
    [JsonConverter(typeof(NullableUInt64Converter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? Deny { get; set; }
}