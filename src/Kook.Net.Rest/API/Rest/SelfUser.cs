using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class SelfUser : User
{
    [JsonPropertyName("mobile_verified")] public bool MobileVerified { get; set; }
    [JsonPropertyName("MobilePrefix")] public string MobilePrefix { get; set; }
    [JsonPropertyName("Mobile")] public string Mobile { get; set; }
    [JsonPropertyName("InvitedCount")] public int? InvitedCount { get; set; }
}
