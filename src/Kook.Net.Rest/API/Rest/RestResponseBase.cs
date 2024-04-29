using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class RestResponseBase
{
    [JsonPropertyName("code")]
    public KookErrorCode Code { get; set; }

    [JsonPropertyName("message")]
    public required string Message { get; set; }

    [JsonPropertyName("data")]
    public required object Data { get; set; }
}
