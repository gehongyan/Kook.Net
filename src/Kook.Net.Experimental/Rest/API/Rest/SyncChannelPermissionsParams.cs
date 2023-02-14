using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class SyncChannelPermissionsParams
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    public SyncChannelPermissionsParams(ulong channelId)
    {
        ChannelId = channelId;
    }
}
