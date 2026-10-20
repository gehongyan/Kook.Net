using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Concentus;
using Kook.Audio.Streams;
using Kook.Audio.WebRtc.Streams;
using Xunit;

namespace Kook.Audio.WebRtc;

/// <summary>
///     Tests for <see cref="WebRtcOpusEncodeStream"/> and <see cref="WebRtcOpusDecodeStream"/>.
/// </summary>
[Trait("Category", "Unit")]
public class WebRtcOpusStreamTests
{
    /// <summary> Collects the payloads written to it. </summary>
    private sealed class CollectingStream : AudioOutStream
    {
        public List<byte[]> Writes { get; } = [];

        public List<(ushort Sequence, uint Timestamp, bool Missed)> Headers { get; } = [];

        public override void WriteHeader(ushort seq, uint timestamp, bool missed) => Headers.Add((seq, timestamp, missed));

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            byte[] copy = new byte[count];
            Buffer.BlockCopy(buffer, offset, copy, 0, count);
            Writes.Add(copy);
            return Task.CompletedTask;
        }
    }

    private static byte[] CreateTone(int frames, double frequency)
    {
        byte[] pcm = new byte[frames * WebRtcAudioFormat.FrameBytes];
        int totalSamples = frames * WebRtcAudioFormat.FrameSamplesPerChannel;
        for (int i = 0; i < totalSamples; i++)
        {
            short value = (short)(Math.Sin(2 * Math.PI * frequency * i / WebRtcAudioFormat.SamplingRate) * 12000);
            // 双声道交错
            BitConverter.GetBytes(value).CopyTo(pcm, i * WebRtcAudioFormat.SampleBytes);
            BitConverter.GetBytes(value).CopyTo(pcm, i * WebRtcAudioFormat.SampleBytes + sizeof(short));
        }
        return pcm;
    }

    [Fact]
    public async Task EncodeStreamProducesOnePacketPerFrame()
    {
        CollectingStream sink = new();
        using WebRtcOpusEncodeStream encoder = new(sink, 32000, AudioApplication.Music, 30);
        byte[] pcm = CreateTone(5, 440);

        await encoder.WriteAsync(pcm, 0, pcm.Length);

        Assert.Equal(5, sink.Writes.Count);
        Assert.All(sink.Writes, packet => Assert.InRange(packet.Length, 1, WebRtcAudioFormat.MaxPacketBytes));
    }

    [Fact]
    public async Task EncodeStreamBuffersPartialFramesAcrossWrites()
    {
        CollectingStream sink = new();
        using WebRtcOpusEncodeStream encoder = new(sink, 32000, AudioApplication.Music, 30);
        byte[] pcm = CreateTone(2, 440);

        // 以不是整帧的块写入，应当只在凑满一帧时才编码
        int chunk = WebRtcAudioFormat.FrameBytes / 3 + 7;
        for (int offset = 0; offset < pcm.Length; offset += chunk)
            await encoder.WriteAsync(pcm, offset, Math.Min(chunk, pcm.Length - offset));

        Assert.Equal(2, sink.Writes.Count);
    }

    [Fact]
    public async Task EncodedPacketsStayWithinTheBitrateBudget()
    {
        CollectingStream sink = new();
        using WebRtcOpusEncodeStream encoder = new(sink, 32000, AudioApplication.Music, 30);
        const int frames = 50;
        byte[] pcm = CreateTone(frames, 440);

        await encoder.WriteAsync(pcm, 0, pcm.Length);

        long totalBytes = 0;
        foreach (byte[] packet in sink.Writes)
            totalBytes += packet.Length;
        double seconds = frames * WebRtcAudioFormat.FrameMillis / 1000.0;
        double kbps = totalBytes * 8 / seconds / 1000;
        // 32 kbps 的目标码率，允许 VBR 的短时波动，但不应显著超过上限
        Assert.InRange(kbps, 1, 32 * 1.2);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void EncodeStreamRejectsInvalidPacketLoss(int packetLoss) =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new WebRtcOpusEncodeStream(new CollectingStream(), 32000, AudioApplication.Music, packetLoss));

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(510001)]
    public void EncodeStreamRejectsInvalidBitrates(int bitrate) =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new WebRtcOpusEncodeStream(new CollectingStream(), bitrate, AudioApplication.Music, 30));

    [Fact]
    public async Task DecodeStreamRestoresTheSamplesOfAnEncodedFrame()
    {
        CollectingStream encoded = new();
        using WebRtcOpusEncodeStream encoder = new(encoded, 64000, AudioApplication.Music, 0);
        byte[] pcm = CreateTone(10, 440);
        await encoder.WriteAsync(pcm, 0, pcm.Length);

        CollectingStream decoded = new();
        using WebRtcOpusDecodeStream decoder = new(decoded);
        ushort sequence = 0;
        foreach (byte[] packet in encoded.Writes)
        {
            decoder.WriteHeader(sequence++, 0, false);
            await decoder.WriteAsync(packet, 0, packet.Length);
        }

        Assert.Equal(encoded.Writes.Count, decoded.Writes.Count);
        Assert.All(decoded.Writes, frame => Assert.Equal(WebRtcAudioFormat.FrameBytes, frame.Length));
        // 编码解码是有损的，不比对样本，而是确认解码出的音频有能量，且不是静音
        short peak = 0;
        foreach (byte[] frame in decoded.Writes)
        {
            for (int i = 0; i < frame.Length; i += sizeof(short))
                peak = Math.Max(peak, Math.Abs(BitConverter.ToInt16(frame, i)));
        }
        Assert.True(peak > 3000, $"The decoded audio is nearly silent (peak: {peak}).");
        Assert.Equal(10, decoded.Headers.Count);
    }

    [Fact]
    public async Task DecodeStreamAcceptsSixtyMillisecondPackets()
    {
        // KOOK 客户端发送的是 60 毫秒的包，原生 Opus 管线的解码缓冲只能容纳 40 毫秒
        IOpusEncoder encoder = OpusCodecFactory.CreateEncoder(WebRtcAudioFormat.SamplingRate, 1,
            Concentus.Enums.OpusApplication.OPUS_APPLICATION_AUDIO);
        encoder.Bitrate = 32000;
        const int sixtyMillis = WebRtcAudioFormat.SamplingRate * 60 / 1000;
        short[] mono = new short[sixtyMillis];
        for (int i = 0; i < mono.Length; i++)
            mono[i] = (short)(Math.Sin(2 * Math.PI * 440 * i / WebRtcAudioFormat.SamplingRate) * 12000);
        byte[] packet = new byte[WebRtcAudioFormat.MaxPacketBytes];
        int length = encoder.Encode(mono, sixtyMillis, packet, packet.Length);

        CollectingStream decoded = new();
        using WebRtcOpusDecodeStream decoder = new(decoded);
        decoder.WriteHeader(1, 0, false);
        await decoder.WriteAsync(packet, 0, length);

        byte[] frame = Assert.Single(decoded.Writes);
        // 60 毫秒的单声道包会被解码成 60 毫秒的双声道 PCM
        Assert.Equal(sixtyMillis * WebRtcAudioFormat.SampleBytes, frame.Length);
    }

    [Fact]
    public async Task DecodeStreamRejectsAPayloadWithoutAHeader()
    {
        using WebRtcOpusDecodeStream decoder = new(new CollectingStream());

        await Assert.ThrowsAsync<InvalidOperationException>(() => decoder.WriteAsync(new byte[10], 0, 10));
    }

    [Fact]
    public void DecodeStreamRejectsAHeaderWithoutAPayload()
    {
        using WebRtcOpusDecodeStream decoder = new(new CollectingStream());
        decoder.WriteHeader(1, 0, false);

        Assert.Throws<InvalidOperationException>(() => decoder.WriteHeader(2, 0, false));
    }
}
