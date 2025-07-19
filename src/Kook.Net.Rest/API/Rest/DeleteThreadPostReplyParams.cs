using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class DeleteThreadPostReplyParams
{
    [JsonPropertyName("channel_id")]
    public required ulong ChannelId { get; set; }

    [JsonPropertyName("thread_id")]
    public ulong? ThreadId { get; set; }

    [JsonPropertyName("post_id")]
    public ulong? PostId { get; set; }
}
