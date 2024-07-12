using Kook.Audio;
using Kook.WebSocket;
using Microsoft.Extensions.Hosting;

namespace Kook.Net.Samples.Audio.Services;

public class MusicService : IHostedService
{
    private readonly Dictionary<ulong, MusicClient> _musicClients = [];

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken) =>
        await Task.WhenAll(_musicClients.Select(x => x.Value.StopAsync()));

    public async Task ConnectAsync(SocketVoiceChannel voiceChannel)
    {
        if (voiceChannel.AudioClient is not null)
            throw new InvalidOperationException("I'm already connected to this voice channel.");
        IAudioClient? audioClient = await voiceChannel.ConnectAsync();
        if (audioClient is null)
            throw new InvalidOperationException("Failed to connect to the voice channel.");
        _musicClients[voiceChannel.Id] = new MusicClient(voiceChannel, audioClient);
    }

    public async Task DisconnectAsync(SocketVoiceChannel voiceChannel)
    {
        if (_musicClients.Remove(voiceChannel.Id, out MusicClient? musicClient))
            await musicClient.StopAsync();
        await voiceChannel.DisconnectAsync();
    }

    public void Enqueue(SocketVoiceChannel voiceChannel, Uri uri) => Enqueue(voiceChannel, [uri]);

    public void Enqueue(SocketVoiceChannel voiceChannel, IEnumerable<Uri> uris)
    {
        if (!_musicClients.TryGetValue(voiceChannel.Id, out MusicClient? musicClient))
            throw new InvalidOperationException("I'm not connected to this voice channel.");
        foreach (Uri uri in uris)
            musicClient.Enqueue(uri);
    }

    public void Skip(SocketVoiceChannel voiceChannel)
    {
        if (!_musicClients.TryGetValue(voiceChannel.Id, out MusicClient? musicClient))
            throw new InvalidOperationException("I'm not connected to this voice channel.");
        musicClient.Skip();
    }

    public IEnumerable<Uri> GetQueue(SocketVoiceChannel voiceChannel)
    {
        if (!_musicClients.TryGetValue(voiceChannel.Id, out MusicClient? musicClient))
            throw new InvalidOperationException("I'm not connected to this voice channel.");
        return musicClient.Queue;
    }
}
