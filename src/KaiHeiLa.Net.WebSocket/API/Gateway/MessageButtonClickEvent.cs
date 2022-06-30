using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Gateway;

internal class MessageButtonClickEvent
{
    [JsonPropertyName("value")]
    public string Value { get; set; }
    
    [JsonPropertyName("msg_id")] public Guid MessageId { get; set; }
    
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }
    
    [JsonPropertyName("target_id")]
    public ulong ChannelId { get; set; }
    
    [JsonPropertyName("user_info")]
    public User User { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }
}