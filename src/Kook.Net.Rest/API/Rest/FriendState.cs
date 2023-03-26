using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Rest;

internal class FriendState
{
    [JsonPropertyName("id")]
    public uint Id { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(FriendStateConverter))]
    private Kook.FriendState State { get; set; }

    [JsonPropertyName("friend_info")]
    public User User { get; set; }

    [JsonPropertyName("own")]
    public bool Own { get; set; }
}
