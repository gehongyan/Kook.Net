using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class RestResponseBase
{
    [JsonPropertyName("code")]
    public KookErrorCode Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("data")]
    public object Data { get; set; }
}
