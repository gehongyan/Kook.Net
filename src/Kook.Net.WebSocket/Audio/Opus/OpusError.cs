namespace Kook.Audio;

/// <summary>
///     表示 Opus 错误代码。
/// </summary>
internal enum OpusError
{
    /// <summary>
    ///     <c>OPUS_OK</c>
    /// </summary>
    /// <remarks>
    ///     没有错误。
    /// </remarks>
    OK = 0,

    /// <summary>
    ///     <c>OPUS_BAD_ARG</c>
    /// </summary>
    /// <remarks>
    ///     一个或多个参数无效/超出范围。
    /// </remarks>
    BadArg = -1,

    /// <summary>
    ///     <c>OPUS_BUFFER_TOO_SMALL</c>
    /// </summary>
    /// <remarks>
    ///     缓冲区中分配的字节不足。
    /// </remarks>
    BufferTooSmall = -2,

    /// <summary>
    ///     <c>OPUS_INTERNAL_ERROR</c>
    /// </summary>
    /// <remarks>
    ///     检测到内部错误。
    /// </remarks>
    InternalError = -3,

    /// <summary>
    ///     <c>OPUS_INVALID_PACKET</c>
    /// </summary>
    /// <remarks>
    ///     传递的压缩数据已损坏。
    /// </remarks>
    InvalidPacket = -4,

    /// <summary>
    ///     <c>OPUS_UNIMPLEMENTED</c>
    /// </summary>
    /// <remarks>
    ///     请求号无效/不支持。
    /// </remarks>
    Unimplemented = -5,

    /// <summary>
    ///     <c>OPUS_INVALID_STATE</c>
    /// </summary>
    /// <remarks>
    ///     编码器或解码器结构无效或已释放。
    /// </remarks>
    InvalidState = -6,

    /// <summary>
    ///     <c>OPUS_ALLOC_FAIL</c>
    /// </summary>
    /// <remarks>
    ///     内存分配失败。
    /// </remarks>
    AllocFail = -7
}
