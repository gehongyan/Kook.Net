using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Gateway;

internal class ChannelBatchUpdateEvent
{
    [JsonPropertyName("added_channel")]
    [JsonConverter(typeof(NullableChannelConverter))]
    public Channel? AddedChannel { get; set; }

    [JsonPropertyName("updated_channel")]
    public required Channel[] UpdatedChannels { get; set; }
}
