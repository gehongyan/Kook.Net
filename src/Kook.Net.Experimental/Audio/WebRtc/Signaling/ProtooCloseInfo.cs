namespace Kook.Audio.WebRtc.Signaling;

internal readonly record struct ProtooCloseInfo(int? Code, string? Reason, Exception? Error);
