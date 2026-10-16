using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

internal class VoiceSeat
{
    [JsonPropertyName("seatId")]
    public int SeatId { get; set; }

    [JsonPropertyName("pauseMic")]
    public bool PauseMic { get; set; }
}
