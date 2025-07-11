using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateThreadParams
{
    [JsonPropertyName("channel_id")]
    public required ulong ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public required ulong GuildId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("category_id")]
    public ulong? ThreadCategoryId { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("cover")]
    public string? Cover { get; set; }

    [JsonPropertyName("content")]
    public required string Content { get; set; }
}
