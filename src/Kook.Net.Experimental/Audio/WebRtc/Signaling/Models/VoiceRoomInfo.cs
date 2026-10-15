using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

internal class VoiceRoomInfo
{
    [JsonPropertyName("roomId")]
    public required string RoomId { get; set; }

    // 0 为自由模式，其余值的含义尚未验证
    [JsonPropertyName("mode")]
    public int Mode { get; set; }

    // 以下三个列表在实测中均为空数组，元素类型尚未确认，因此保留为原始 JSON
    [JsonPropertyName("speakList")]
    public IReadOnlyList<JsonElement>? SpeakList { get; set; }

    [JsonPropertyName("applyList")]
    public IReadOnlyList<JsonElement>? ApplyList { get; set; }

    [JsonPropertyName("inviteList")]
    public IReadOnlyList<JsonElement>? InviteList { get; set; }

    [JsonPropertyName("speakTime")]
    public int? SpeakTime { get; set; }

    [JsonPropertyName("applyTimeout")]
    public int? ApplyTimeout { get; set; }

    [JsonPropertyName("inviteTimeout")]
    public int? InviteTimeout { get; set; }

    [JsonPropertyName("maxSpeakers")]
    public int? MaxSpeakers { get; set; }

    [JsonPropertyName("seatList")]
    public IReadOnlyList<VoiceSeat>? SeatList { get; set; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? ExtraData { get; set; }
}
