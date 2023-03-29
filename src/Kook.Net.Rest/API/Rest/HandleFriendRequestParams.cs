using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Rest;

internal class HandleFriendRequestParams
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("accept")]
    [JsonConverter(typeof(NumberBooleanConverter))]
    public bool HandleResult { get; set; }
}
