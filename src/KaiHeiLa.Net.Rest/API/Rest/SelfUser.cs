using System.Text.Json.Serialization;

namespace KaiHeiLa.API;

internal class SelfUser : User
{
    [JsonPropertyName("mobile_verified")] public bool MobileVerified { get; set; }
    [JsonPropertyName("MobilePrefix")] public string MobilePrefix { get; set; }
    [JsonPropertyName("Mobile")] public string Mobile { get; set; }
    [JsonPropertyName("InvitedCount")] public int? InvitedCount { get; set; }
}