using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Rest;

internal class GuildMember : User
{
    [JsonPropertyName("nickname")] public string Nickname { get; set; }
    [JsonPropertyName("mobile_verified")] public bool MobileVerified { get; set; }

    [JsonPropertyName("joined_at")]
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset JoinedAt { get; set; }

    [JsonPropertyName("active_time")]
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset ActiveAt { get; set; }

    [JsonPropertyName("hoist_info")] public HoistInfo HoistInfo { get; set; }
    [JsonPropertyName("color")] public uint Color { get; set; }
    [JsonPropertyName("roles")] public uint[] Roles { get; set; }

    [JsonPropertyName("is_master")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsOwner { get; set; }

    [JsonPropertyName("desc")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Description { get; set; }
}

internal class HoistInfo
{
    [JsonPropertyName("role_id")] public uint RoleId { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("color")] public uint Color { get; set; }
}