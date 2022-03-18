using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class Channel
{
    [JsonPropertyName("id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("user_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong CreateUserId { get; set; }

    [JsonPropertyName("guild_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong GuildId { get; set; }

    [JsonPropertyName("is_category")] public bool IsCategory { get; set; }

    [JsonPropertyName("parent_id")]
    [JsonConverter(typeof(NullableUInt64Converter))]
    public ulong? CategoryId { get; set; }

    [JsonPropertyName("level")] public int Position { get; set; }
    [JsonPropertyName("type")] public ChannelType Type { get; set; }

    [JsonPropertyName("permission_overwrites")]
    public RolePermissionOverwrite[] RolePermissionOverwrites { get; set; }

    [JsonPropertyName("permission_users")] 
    public UserPermissionOverwrite[] UserPermissionOverwrites { get; set; }

    [JsonPropertyName("permission_sync")] public int PermissionSync { get; set; }
    
    // Text
    [JsonPropertyName("topic")] public string Topic { get; set; }
    [JsonPropertyName("slow_mode")] public int SlowMode { get; set; }
    
    // Voice
    [JsonPropertyName("limit_amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? UserLimit { get; set; }

    [JsonPropertyName("voice_quality")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VoiceQuality? VoiceQuality { get; set; }

    [JsonPropertyName("server_url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string ServerUrl { get; set; }
}