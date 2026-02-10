using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class JoinRestrictions
{
    [JsonPropertyName("disable_violation")]
    public bool DisableViolation { get; set; }

    [JsonPropertyName("disable_unverified")]
    public bool DisableUnverified { get; set; }

    [JsonPropertyName("disable_unverified_and_violation")]
    public bool DisableUnverifiedAndViolation { get; set; }
}
