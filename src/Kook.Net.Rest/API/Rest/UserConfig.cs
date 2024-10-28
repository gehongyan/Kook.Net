using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class UserConfig
{
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }
}
