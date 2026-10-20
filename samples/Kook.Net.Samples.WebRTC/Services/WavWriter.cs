using System.Runtime.InteropServices;
using System.Text;

namespace Kook.Net.Samples.WebRTC.Services;

/// <summary>
///     将 16 位 PCM 写入 WAV 文件，关闭时回填头部的长度字段。
/// </summary>
public sealed class WavWriter : IDisposable
{
    private const int HeaderSize = 44;

    private readonly FileStream _stream;
    private readonly int _sampleRate;
    private readonly int _channels;
    private long _dataBytes;
    private bool _disposed;

    public WavWriter(string path, int sampleRate, int channels)
    {
        _sampleRate = sampleRate;
        _channels = channels;
        _stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
        _stream.Write(new byte[HeaderSize], 0, HeaderSize);
    }

    public string Path => _stream.Name;

    public double DurationSeconds => (double)_dataBytes / (_sampleRate * _channels * sizeof(short));

    public void Write(byte[] pcm, int offset, int count)
    {
        if (_disposed || count <= 0)
            return;
        _stream.Write(pcm, offset, count);
        _dataBytes += count;
    }

    public void WriteSilence(int samplesPerChannel)
    {
        if (_disposed || samplesPerChannel <= 0)
            return;
        byte[] zeros = new byte[Math.Min(samplesPerChannel, _sampleRate) * _channels * sizeof(short)];
        int remaining = samplesPerChannel;
        while (remaining > 0)
        {
            int chunk = Math.Min(remaining, zeros.Length / (_channels * sizeof(short)));
            Write(zeros, 0, chunk * _channels * sizeof(short));
            remaining -= chunk;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        _stream.Seek(0, SeekOrigin.Begin);
        using (BinaryWriter writer = new(_stream, Encoding.ASCII, leaveOpen: true))
        {
            writer.Write(Encoding.ASCII.GetBytes("RIFF"));
            writer.Write((int)(36 + _dataBytes));
            writer.Write(Encoding.ASCII.GetBytes("WAVE"));
            writer.Write(Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)_channels);
            writer.Write(_sampleRate);
            writer.Write(_sampleRate * _channels * sizeof(short));
            writer.Write((short)(_channels * sizeof(short)));
            writer.Write((short)16);
            writer.Write(Encoding.ASCII.GetBytes("data"));
            writer.Write((int)_dataBytes);
        }
        _stream.Dispose();
    }
}
