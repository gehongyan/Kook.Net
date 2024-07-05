using System.Collections.Concurrent;
using System.Diagnostics;
using Kook.Audio;
using Kook.WebSocket;
using Microsoft.Extensions.Hosting;

namespace Kook.Net.Samples.Audio.Services;

public class MusicService : IHostedService
{
    private readonly BlockingCollection<Uri> _musicQueue = [];
    private ISocketMessageChannel? _sourceChannel;
    private IAudioClient? _audioClient;
    private Process? _ffmpeg;

    public IEnumerable<Uri> Queue => _musicQueue;

    public Uri? CurrentPlaying { get; private set; }

    public void SetAudioClient(ISocketMessageChannel sourceChannel, IAudioClient audioClient)
    {
        _sourceChannel = sourceChannel;
        _audioClient = audioClient;
    }

    public void Enqueue(Uri source)
    {
        _musicQueue.Add(source);
    }

    public void Skip()
    {
        _ffmpeg?.Kill();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (_musicQueue.TryTake(out Uri? source, Timeout.Infinite, cancellationToken))
            await PlayAsync(source, cancellationToken);
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _musicQueue.Dispose();
        _ffmpeg?.Kill();
        return Task.CompletedTask;
    }

    private async Task PlayAsync(Uri source, CancellationToken cancellationToken)
    {
        if (_audioClient?.ConnectionState != ConnectionState.Connected
            || _sourceChannel is null)
            return;
        CurrentPlaying = source;
        _ = _sourceChannel.SendTextAsync($"Now playing {source}");
        using Process? ffmpeg = Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"""-hide_banner -loglevel panic -i "{source}" -ac 2 -f s16le -ar 48000 pipe:1""",
            UseShellExecute = false,
            RedirectStandardOutput = true,
        });
        _ffmpeg = ffmpeg;
        if (ffmpeg is null) return;
        await using Stream output = ffmpeg.StandardOutput.BaseStream;
        await using AudioOutStream kook = _audioClient.CreatePcmStream(AudioApplication.Music);
        try
        {
            await output.CopyToAsync(kook, cancellationToken);
        }
        finally
        {
            await kook.FlushAsync(cancellationToken);
        }
    }
}
