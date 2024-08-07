using System.Collections.Concurrent;
using System.Diagnostics;
using Kook.Audio;
using Kook.WebSocket;

namespace Kook.Net.Samples.Audio.Services;

public class MusicClient
{
    private readonly BlockingCollection<Uri> _musicQueue = [];
    private readonly SocketVoiceChannel _voiceChannel;
    private readonly IAudioClient _audioClient;
    private int? _ffmpegProcessId;
    private readonly CancellationTokenSource _cancellationToken = new();

    public IEnumerable<Uri> Queue => _musicQueue;

    public Uri? CurrentPlaying { get; private set; }

    public MusicClient(SocketVoiceChannel voiceChannel, IAudioClient audioClient)
    {
        _voiceChannel = voiceChannel;
        _audioClient = audioClient;
        _ = Task.Factory.StartNew(StartAsync, TaskCreationOptions.LongRunning);
    }

    public void Enqueue(Uri source)
    {
        _musicQueue.Add(source);
    }

    public void Skip() => KillCurrentProcess();

    private void KillCurrentProcess()
    {
        if (!_ffmpegProcessId.HasValue) return;
        try
        {
            int processIdToKill = _ffmpegProcessId.Value;
            _ffmpegProcessId = null;
            Process process = Process.GetProcessById(processIdToKill);
            process.Kill();
        }
        catch (ArgumentException)
        {
            // ignored
        }
    }

    public async Task StartAsync()
    {
        await Task.Yield();
        try
        {
            foreach (Uri source in _musicQueue.GetConsumingEnumerable(_cancellationToken.Token))
                await PlayAsync(source, _cancellationToken.Token);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task StopAsync()
    {
        await _cancellationToken.CancelAsync();
        _musicQueue.Dispose();
        KillCurrentProcess();
    }

    private async Task PlayAsync(Uri source, CancellationToken cancellationToken)
    {
        if (_audioClient?.ConnectionState != ConnectionState.Connected
            || _voiceChannel is null)
            return;
        CurrentPlaying = source;
        _ = _voiceChannel.SendTextAsync($"Now playing {source}");
        using Process? ffmpeg = Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"""-hide_banner -loglevel panic -i "{source}" -ac 2 -f s16le -ar 48000 -""",
            UseShellExecute = false,
            RedirectStandardOutput = true,
        });
        if (ffmpeg is null) return;
        _ffmpegProcessId = ffmpeg.Id;
        await using Stream output = ffmpeg.StandardOutput.BaseStream;
        await using AudioOutStream kook = _audioClient.CreateDirectPcmStream(AudioApplication.Music);
        try
        {
            await output.CopyToAsync(kook, cancellationToken);
        }
        finally
        {
            await kook.FlushAsync(cancellationToken);
            CurrentPlaying = null;
        }
    }
}
