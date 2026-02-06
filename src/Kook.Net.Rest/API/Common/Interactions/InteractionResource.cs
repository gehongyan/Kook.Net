using System.Text.Json.Serialization;

namespace Kook.API;

internal class InteractionResource
{
    [JsonPropertyName("emoji_id")]
    public required InteractiveEmoteType EmojiId { get; set; }

    [JsonPropertyName("interact_type")]
    public required int InteractType { get; set; }

    [JsonPropertyName("dynamic_url")]
    public required string DynamicUrl { get; set; }

    [JsonPropertyName("first_dynamic_urls")]
    public required string[] FirstDynamicUrls { get; set; }

    [JsonPropertyName("first_dynamic_duration")]
    public required int FirstDynamicDuration { get; set; }

    [JsonPropertyName("dynamic_urls")]
    public required string[] DynamicUrls { get; set; }

    [JsonPropertyName("dynamic_duration")]
    public required int DynamicDuration { get; set; }

    [JsonPropertyName("result")]
    public required int[] Result { get; set; }

    [JsonPropertyName("result_img")]
    public required string[] ResultImg { get; set; }
}
