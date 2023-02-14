using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class Channel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("user_id")]
    public ulong CreatorId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    // [JsonPropertyName("is_category")] 
    // public bool IsCategory { get; set; } // TODO: Bool when API but int when event

    [JsonPropertyName("parent_id")]
    [JsonConverter(typeof(NullableUInt64Converter))]
    public ulong? CategoryId { get; set; }

    [JsonPropertyName("level")] public int? Position { get; set; }
    [JsonPropertyName("type")] public ChannelType Type { get; set; }

    [JsonPropertyName("permission_overwrites")]
    public RolePermissionOverwrite[] RolePermissionOverwrites { get; set; }

    [JsonPropertyName("permission_users")]
    public UserPermissionOverwrite[] UserPermissionOverwrites { get; set; }

    [JsonPropertyName("channels")] public Channel[] Channels { get; set; }

    [JsonPropertyName("permission_sync")]
    [JsonConverter(typeof(NullableNumberBooleanConverter))]
    public bool? PermissionSync { get; set; }

    // Text
    [JsonPropertyName("topic")] public string Topic { get; set; }
    [JsonPropertyName("slow_mode")] public int SlowMode { get; set; }

    // Voice
    [JsonPropertyName("limit_amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? UserLimit { get; set; }

    [JsonPropertyName("voice_quality")]
    [JsonConverter(typeof(NullableVoiceQualityConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VoiceQuality? VoiceQuality { get; set; }

    [JsonPropertyName("server_url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string ServerUrl { get; set; }

    [JsonPropertyName("has_password")]
    public bool HasPassword { get; set; }
}
