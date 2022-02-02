using System.Text.Json.Serialization;

namespace KaiHeiLa.API;

internal class User
{
    [JsonPropertyName("id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public uint Id { get; set; }

    [JsonPropertyName("username")] public string Username { get; set; }
    [JsonPropertyName("nickname")] public string Nickname { get; set; }
    [JsonPropertyName("identify_num")] public string IdentifyNum { get; set; }
    [JsonPropertyName("online")] public bool Online { get; set; }
    [JsonPropertyName("bot")] public bool Bot { get; set; }
    [JsonPropertyName("status")] public int Status { get; set; }
    [JsonPropertyName("avatar")] public string Avatar { get; set; }
    [JsonPropertyName("vip_avatar")] public string VipAvatar { get; set; }
    [JsonPropertyName("mobile_verified")] public bool MobileVerified { get; set; }
    [JsonPropertyName("roles")] public uint[] Roles { get; set; }
}