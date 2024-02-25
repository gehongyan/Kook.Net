namespace Kook.Audio;

/// <summary>
///     Represents the Opus error codes.
/// </summary>
internal enum OpusError
{
    /// <summary>
    ///     <c>OPUS_OK</c>
    /// </summary>
    /// <remarks>
    ///     No error.
    /// </remarks>
    OK = 0,

    /// <summary>
    ///     <c>OPUS_BAD_ARG</c>
    /// </summary>
    /// <remarks>
    ///     One or more invalid/out of range arguments.
    /// </remarks>
    BadArg = -1,

    /// <summary>
    ///     <c>OPUS_BUFFER_TOO_SMALL</c>
    /// </summary>
    /// <remarks>
    ///     Not enough bytes allocated in the buffer.
    /// </remarks>
    BufferTooSmall = -2,

    /// <summary>
    ///     <c>OPUS_INTERNAL_ERROR</c>
    /// </summary>
    /// <remarks>
    ///     An internal error was detected.
    /// </remarks>
    InternalError = -3,

    /// <summary>
    ///     <c>OPUS_INVALID_PACKET</c>
    /// </summary>
    /// <remarks>
    ///     The compressed data passed is corrupted.
    /// </remarks>
    InvalidPacket = -4,

    /// <summary>
    ///     <c>OPUS_UNIMPLEMENTED</c>
    /// </summary>
    /// <remarks>
    ///     Invalid/unsupported request number.
    /// </remarks>
    Unimplemented = -5,

    /// <summary>
    ///     <c>OPUS_INVALID_STATE</c>
    /// </summary>
    /// <remarks>
    ///     An encoder or decoder structure is invalid or already freed.
    /// </remarks>
    InvalidState = -6,

    /// <summary>
    ///     <c>OPUS_ALLOC_FAIL</c>
    /// </summary>
    /// <remarks>
    ///     Memory allocation has failed.
    /// </remarks>
    AllocFail = -7
}
