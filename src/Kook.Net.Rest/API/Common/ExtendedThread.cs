using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class ExtendedThread : Thread
{
    [JsonPropertyName("latest_active_time")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset LatestActiveTime { get; set; }

    [JsonPropertyName("create_time")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset CreateTime { get; set; }

    [JsonPropertyName("is_updated")]
    public bool IsUpdated { get; set; }

    [JsonPropertyName("content_deleted")]
    public bool ContentDeleted { get; set; }

    [JsonPropertyName("content_deleted_type")]
    public ThreadContentDeletedBy ContentDeletedType { get; set; }

    [JsonPropertyName("collect_num")]
    public int CollectNum { get; set; }

    [JsonPropertyName("post_count")]
    public int PostCount { get; set; }
}
