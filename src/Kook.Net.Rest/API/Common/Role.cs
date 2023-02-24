using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class Role
{
    [JsonPropertyName("role_id")]
    public uint Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("color")]
    [JsonConverter(typeof(RawValueColorConverter))]
    public Color Color { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("hoist")]
    public int Hoist { get; set; }

    [JsonPropertyName("mentionable")]
    public int Mentionable { get; set; }

    [JsonPropertyName("permissions")]
    public ulong Permissions { get; set; }

    [JsonPropertyName("type")]
    public RoleType? Type { get; set; }
}
