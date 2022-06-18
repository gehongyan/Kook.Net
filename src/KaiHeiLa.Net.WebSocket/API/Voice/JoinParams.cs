using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Voice;

internal class JoinParams
{
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; }
}