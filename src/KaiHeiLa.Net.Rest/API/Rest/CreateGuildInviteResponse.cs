using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class CreateGuildInviteResponse
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}