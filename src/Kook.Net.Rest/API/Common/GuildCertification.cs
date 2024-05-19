using System.Text.Json.Serialization;

namespace Kook.API;

internal class GuildCertification
{
    [JsonPropertyName("type")]
    public GuildCertificationType Type { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("pic")]
    public required string Picture { get; set; }

    [JsonPropertyName("desc")]
    public required string Description { get; set; }
}
