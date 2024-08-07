namespace Kook.Audio;

/// <summary>
///     Represents an RTP frame.
/// </summary>
public readonly struct RtpFrame
{
    /// <summary>
    ///     Gets the sequence number of this frame.
    /// </summary>
    public ushort Sequence { get; init; }

    /// <summary>
    ///     Gets the timestamp of this frame.
    /// </summary>
    public uint Timestamp { get; init; }

    /// <summary>
    ///     Gets the payload of this frame.
    /// </summary>
    public byte[] Payload { get; init; }

    /// <summary>
    ///     Gets whether this frame was missed.
    /// </summary>
    public bool Missed { get; init; }

    /// <summary>
    ///     Initializes a new instance of <see cref="RtpFrame"/>.
    /// </summary>
    public RtpFrame(ushort sequence, uint timestamp, byte[] payload, bool missed)
    {
        Sequence = sequence;
        Timestamp = timestamp;
        Payload = payload;
        Missed = missed;
    }
}
