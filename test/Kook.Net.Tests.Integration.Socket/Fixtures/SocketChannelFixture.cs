using System.Threading.Tasks;
using Kook.WebSocket;

namespace Kook;

public class SocketChannelFixture : SocketGuildFixture
{
    private const string TextChannelName = "TEST TEXT CHANNEL";
    private const string VoiceChannelName = "TEST VOICE CHANNEL";

    private readonly TaskCompletionSource<SocketTextChannel> _textChannelPromise = new();
    private readonly TaskCompletionSource<SocketVoiceChannel> _voiceChannelPromise = new();

    public SocketTextChannel TextChannel { get; private set; } = null!;

    public SocketVoiceChannel VoiceChannel { get; private set; } = null!;

    public SocketChannelFixture()
    {
        InitializeAsync().GetAwaiter().GetResult();
    }

    private async Task InitializeAsync()
    {
        Client.ChannelCreated += OnChannelCreated;
        await Guild.CreateTextChannelAsync(TextChannelName);
        await Guild.CreateVoiceChannelAsync(VoiceChannelName);
        TextChannel = await _textChannelPromise.Task.WithTimeout();
        VoiceChannel = await _voiceChannelPromise.Task.WithTimeout();
        Client.ChannelCreated -= OnChannelCreated;
    }

    private Task OnChannelCreated(SocketChannel arg)
    {
        if (arg is SocketVoiceChannel { Name: VoiceChannelName } voiceChannel)
            _voiceChannelPromise.SetResult(voiceChannel);
        else if (arg is SocketTextChannel { Name: TextChannelName } textChannel)
            _textChannelPromise.SetResult(textChannel);
        return Task.CompletedTask;
    }

    public override async ValueTask DisposeAsync()
    {
        await TextChannel.DeleteAsync();
        await VoiceChannel.DeleteAsync();
        await base.DisposeAsync();
    }

    /// <inheritdoc />
    public override void Dispose() => DisposeAsync().AsTask();
}
