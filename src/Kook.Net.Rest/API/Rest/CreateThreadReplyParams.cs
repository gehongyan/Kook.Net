using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateThreadReplyParams
{
    [JsonPropertyName("channel_id")]
    public required ulong ChannelId { get; set; }

    [JsonPropertyName("thread_id")]
    public required ulong ThreadId { get; set; }

    [JsonPropertyName("reply_id")]
    public ulong? ReplyId { get; set; }

    [JsonPropertyName("content")]
    public required string Content { get; set; }
}
