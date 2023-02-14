using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateGuildInviteResponse
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}
