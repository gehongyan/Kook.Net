using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class GetChannelPermissionOverwritesResponse
{    
    [JsonPropertyName("permission_overwrites")]
    public RolePermissionOverwrite[] RolePermissionOverwrites { get; set; }

    [JsonPropertyName("permission_users")] 
    public UserPermissionOverwrite[] UserPermissionOverwrites { get; set; }

    [JsonPropertyName("permission_sync")] public int PermissionSync { get; set; }
}