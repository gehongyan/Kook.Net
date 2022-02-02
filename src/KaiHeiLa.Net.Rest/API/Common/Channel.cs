using System.Text.Json.Serialization;

namespace KaiHeiLa.API;

internal class Channel
{
    [JsonPropertyName("id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("user_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public uint UserId { get; set; }

    [JsonPropertyName("guild_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong GuildId { get; set; }

    [JsonPropertyName("topic")] public string Topic { get; set; }
    [JsonPropertyName("is_category")] public bool IsCategory { get; set; }

    [JsonPropertyName("parent_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong ParentId { get; set; }

    [JsonPropertyName("level")] public int Level { get; set; }
    [JsonPropertyName("slow_mode")] public int SlowMode { get; set; }
    [JsonPropertyName("type")] public int Type { get; set; }

    [JsonPropertyName("permission_overwrites")]
    public RolePermissionOverwrite[] PermissionOverwrites { get; set; }

    [JsonPropertyName("permission_users")] 
    public UserPermissionOverwrite[] PermissionUsers { get; set; }

    [JsonPropertyName("permission_sync")] public int PermissionSync { get; set; }
}