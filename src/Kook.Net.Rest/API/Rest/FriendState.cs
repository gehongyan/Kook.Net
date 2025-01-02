using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Rest;

internal class FriendState
{
    [JsonPropertyName("id")]
    public uint Id { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(FriendStateConverter))]
    public Kook.FriendState Type { get; set; }

    [JsonPropertyName("relation_type")]
    [JsonConverter(typeof(IntimacyRelationTypeConverter))]
    public IntimacyRelationType? IntimacyType { get; set; }

    [JsonPropertyName("relation_time")]
    [JsonConverter(typeof(NullableDateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset? RelationTime { get; set; }

    [JsonPropertyName("relation_status")]
    [JsonConverter(typeof(IntimacyStateConverter))]
    public IntimacyState? IntimacyState { get; set; }

    [JsonPropertyName("friend_status")]
    [JsonConverter(typeof(FriendStateConverter))]
    public int FriendStatus { get; set; }

    [JsonPropertyName("friend_info")]
    public required User User { get; set; }

    [JsonPropertyName("own")]
    public bool Own { get; set; }
}
