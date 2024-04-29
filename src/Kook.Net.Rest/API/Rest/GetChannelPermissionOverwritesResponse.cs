using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class GetChannelPermissionOverwritesResponse
{
    [JsonPropertyName("permission_overwrites")]
    public required RolePermissionOverwrite[] RolePermissionOverwrites { get; set; }

    [JsonPropertyName("permission_users")]
    public required UserPermissionOverwrite[] UserPermissionOverwrites { get; set; }

    [JsonPropertyName("permission_sync")]
    [JsonConverter(typeof(NumberBooleanConverter))]
    public bool PermissionSync { get; set; }
}
