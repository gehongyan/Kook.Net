using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class MentionRole
{
    [JsonPropertyName("role_id")] public uint RoleId { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("color")]
    [JsonConverter(typeof(ColorConverter))]
    public Color Color { get; set; }

    [JsonPropertyName("position")] public int Position { get; set; }
    [JsonPropertyName("hoist")] public int Hoist { get; set; }
    [JsonPropertyName("mentionable")] public int Mentionable { get; set; }
    [JsonPropertyName("permissions")] public ulong Permissions { get; set; }
}