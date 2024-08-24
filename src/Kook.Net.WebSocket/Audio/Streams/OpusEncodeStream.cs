namespace Kook.Audio.Streams;

///<summary>
///     表示一个 Opus 编码音频流。
/// </summary>
public class OpusEncodeStream : AudioOutStream
{
    /// <summary>
    ///     获取音频流的采样率。
    /// </summary>
    public const int SampleRate = 48000;

    private readonly AudioStream _next;
    private readonly OpusEncoder _encoder;
    private readonly byte[] _buffer;
    private int _partialFramePos;
    private ushort _seq;
    private uint _timestamp;

    /// <summary>
    ///     初始化一个 <see cref="OpusEncodeStream"/> 类的新实例。
    /// </summary>
    /// <param name="next"> 要写入编码数据的音频流，是音频流写入链中的下一个音频流对象。 </param>
    /// <param name="bitrate"> 音频流的比特率。 </param>
    /// <param name="application"> 音频应用程序的应用场景。 </param>
    /// <param name="packetLoss"> 音频流的丢包率。 </param>
    public OpusEncodeStream(AudioStream next, int bitrate, AudioApplication application, int packetLoss)
    {
        _next = next;
        _encoder = new OpusEncoder(bitrate, application, packetLoss);
        _buffer = new byte[OpusConverter.FrameBytes];
    }

    /// <summary>
    ///     发送静默帧以避免数据传输中断后的插值错误。
    /// </summary>
    /// <returns> 一个表示写入静默帧操作的异步任务。 </returns>
    public async Task WriteSilentFramesAsync()
    {
        byte[] frameBytes = new byte[OpusConverter.FrameBytes];

        // Magic silence numbers.
        frameBytes[0] = 0xF8;
        frameBytes[1] = 0xFF;
        frameBytes[2] = 0xFE;

        // The rest of the array is already zeroes, so no need to fill the rest.

        const int frameCount = 5;
        for (int i = 0; i < frameCount; i += 1)
        {
            await WriteAsync(frameBytes, 0, frameBytes.Length).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        //Assume thread-safe
        while (count > 0)
        {
            if (_partialFramePos == 0 && count >= OpusConverter.FrameBytes)
            {
                //We have enough data and no partial frames. Pass the buffer directly to the encoder
                int encFrameSize = _encoder.EncodeFrame(buffer, offset, _buffer, 0);
                _next.WriteHeader(_seq, _timestamp, false);
                await _next.WriteAsync(_buffer, 0, encFrameSize, cancellationToken).ConfigureAwait(false);

                offset += OpusConverter.FrameBytes;
                count -= OpusConverter.FrameBytes;
                _seq++;
                _timestamp += OpusConverter.FrameSamplesPerChannel;
            }
            else if (_partialFramePos + count >= OpusConverter.FrameBytes)
            {
                //We have enough data to complete a previous partial frame.
                int partialSize = OpusConverter.FrameBytes - _partialFramePos;
                Buffer.BlockCopy(buffer, offset, _buffer, _partialFramePos, partialSize);
                int encFrameSize = _encoder.EncodeFrame(_buffer, 0, _buffer, 0);
                _next.WriteHeader(_seq, _timestamp, false);
                await _next.WriteAsync(_buffer, 0, encFrameSize, cancellationToken).ConfigureAwait(false);

                offset += partialSize;
                count -= partialSize;
                _partialFramePos = 0;
                _seq++;
                _timestamp += OpusConverter.FrameSamplesPerChannel;
            }
            else
            {
                //Not enough data to build a complete frame, store this part for later
                Buffer.BlockCopy(buffer, offset, _buffer, _partialFramePos, count);
                _partialFramePos += count;
                break;
            }
        }
    }

    /* //Opus throws memory errors on bad frames
    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
        try
        {
            int encFrameSize = _encoder.EncodeFrame(_partialFrameBuffer, 0, _partialFramePos, _buffer, 0);
            base.Write(_buffer, 0, encFrameSize);
        }
        catch (Exception) { } //Incomplete frame
        _partialFramePos = 0;
        await base.FlushAsync(cancellationToken).ConfigureAwait(false);
    }*/

    /// <inheritdoc />
    public override Task FlushAsync(CancellationToken cancellationToken) =>
        _next.FlushAsync(cancellationToken);

    /// <inheritdoc />
    public override Task ClearAsync(CancellationToken cancellationToken) =>
        _next.ClearAsync(cancellationToken);

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _encoder.Dispose();
            _next.Dispose();
        }
        base.Dispose(disposing);
    }
}
