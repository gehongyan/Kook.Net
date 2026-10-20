namespace Kook.Audio.WebRtc.Signaling;

internal sealed class ProtooRequestResult
{
    private ProtooRequestResult(bool accepted, object? data, int? errorCode, string? errorReason)
    {
        Accepted = accepted;
        Data = data;
        ErrorCode = errorCode;
        ErrorReason = errorReason;
    }

    public bool Accepted { get; }

    public object? Data { get; }

    public int? ErrorCode { get; }

    public string? ErrorReason { get; }

    public static ProtooRequestResult Accept(object? data = null) => new(true, data, null, null);

    public static ProtooRequestResult Reject(int errorCode, string errorReason) => new(false, null, errorCode, errorReason);
}
