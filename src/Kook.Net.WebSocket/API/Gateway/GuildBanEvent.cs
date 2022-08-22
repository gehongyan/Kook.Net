using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Gateway;

internal class GuildBanEvent
{
    [JsonPropertyName("operator_id")]
    public ulong OperatorUserId { get; set; }
    
    [JsonPropertyName("user_id")]
    public ulong[] UserIds { get; set; }
}