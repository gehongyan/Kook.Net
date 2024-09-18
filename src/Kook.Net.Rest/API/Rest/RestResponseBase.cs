using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class RestResponseBase
{
    [JsonPropertyName("code")]
    public required KookErrorCode Code { get; set; }

    [JsonPropertyName("message")]
    public required string Message { get; set; }

    [JsonPropertyName("data")]
    public JsonElement? Data { get; set; }
}
