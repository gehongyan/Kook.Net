using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class Role
{
    [JsonPropertyName("role_id")]
    public uint Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("color")]
    [JsonConverter(typeof(RawValueColorConverter))]
    public Color Color { get; set; }

    [JsonPropertyName("color_type")]
    public ColorType ColorType { get; set; }

    [JsonPropertyName("color_map")]
    [JsonConverter(typeof(NullableGradientColorConverter))]
    public GradientColor? GradientColor { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("hoist")]
    [JsonConverter(typeof(NumberBooleanConverter))]
    public bool IsHoisted { get; set; }

    [JsonPropertyName("mentionable")]
    [JsonConverter(typeof(NumberBooleanConverter))]
    public bool IsMentionable { get; set; }

    [JsonPropertyName("permissions")]
    public ulong Permissions { get; set; }

    [JsonPropertyName("type")]
    public RoleType Type { get; set; }
}
