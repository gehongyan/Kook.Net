using System.Text.Json.Serialization;

namespace KaiHeiLa.API;

internal class User
{
    [JsonPropertyName("id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong Id { get; set; }

    [JsonPropertyName("username")] public string Username { get; set; }
    [JsonPropertyName("nickname")] public string Nickname { get; set; }
    [JsonPropertyName("identify_num")] public string IdentifyNumber { get; set; }
    [JsonPropertyName("online")] public bool Online { get; set; }
    [JsonPropertyName("bot")] public bool Bot { get; set; }
    [JsonPropertyName("status")] public int Status { get; set; }
    [JsonPropertyName("avatar")] public string Avatar { get; set; }
    [JsonPropertyName("vip_avatar")] public string VIPAvatar { get; set; }
    [JsonPropertyName("roles")] public uint[] Roles { get; set; }
    [JsonPropertyName("is_vip")] public bool IsVIP { get; set; }
}