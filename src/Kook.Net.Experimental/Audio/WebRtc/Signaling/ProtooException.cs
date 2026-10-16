namespace Kook.Audio.WebRtc.Signaling;

internal sealed class ProtooException : Exception
{
    public ProtooException(string message, int? errorCode = null, string? errorReason = null, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        ErrorReason = errorReason;
    }

    public int? ErrorCode { get; }

    public string? ErrorReason { get; }
}
