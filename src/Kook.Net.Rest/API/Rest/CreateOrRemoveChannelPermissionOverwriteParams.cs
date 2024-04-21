using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateOrRemoveChannelPermissionOverwriteParams
{
    [JsonPropertyName("channel_id")]
    public required ulong ChannelId { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(PermissionOverwriteTargetTypeConverter))]
    public PermissionOverwriteTargetType TargetType { get; set; }

    [JsonPropertyName("value")]
    public required ulong TargetId { get; set; }
}
