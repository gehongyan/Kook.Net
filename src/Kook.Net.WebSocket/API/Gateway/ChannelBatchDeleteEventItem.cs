using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class ChannelBatchDeleteEventItem
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }
}
