using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class GuildMember : User
{
    [JsonPropertyName("mobile_verified")] public bool MobileVerified { get; set; }

    [JsonPropertyName("joined_at")]
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset JoinedAt { get; set; }

    [JsonPropertyName("active_time")]
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset ActiveTime { get; set; }

    [JsonPropertyName("hoist_info")] public HoistInfo HoistInfo { get; set; }
    [JsonPropertyName("color")] public uint Color { get; set; }
    [JsonPropertyName("roles")] public uint[] Roles { get; set; }
}

internal class HoistInfo
{
    [JsonPropertyName("role_id")] public uint RoleId { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("color")] public uint Color { get; set; }
}