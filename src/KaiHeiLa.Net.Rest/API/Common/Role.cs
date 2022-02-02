using System.Text.Json.Serialization;

namespace KaiHeiLa.API;

internal class Role
{
    [JsonPropertyName("role_id")] public uint RoleId { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("color")] public int Color { get; set; }
    [JsonPropertyName("position")] public int Position { get; set; }
    [JsonPropertyName("hoist")] public int Hoist { get; set; }
    [JsonPropertyName("mentionable")] public int Mentionable { get; set; }
    [JsonPropertyName("permissions")] public int Permissions { get; set; }
}