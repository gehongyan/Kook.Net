using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Kook.WebSocket;

namespace Kook;

public class SocketChannelFixture : SocketGuildFixture
{
    private const string TextChannelName = "TEST TEXT CHANNEL";
    private const string VoiceChannelName = "TEST VOICE CHANNEL";

    private readonly TaskCompletionSource<SocketTextChannel> _textChannelCreated = new();
    private readonly TaskCompletionSource<SocketVoiceChannel> _voiceChannelCreated = new();

    public SocketTextChannel TextChannel { get; private set; }

    public SocketVoiceChannel VoiceChannel { get; private set; }

    public SocketChannelFixture()
    {
        InitializeAsync().GetAwaiter().GetResult();
    }

    [MemberNotNull(nameof(TextChannel), nameof(VoiceChannel))]
    private async Task InitializeAsync()
    {
        Client.ChannelCreated += OnChannelCreated;
        await Guild.CreateTextChannelAsync(TextChannelName);
        await Guild.CreateVoiceChannelAsync(VoiceChannelName);
        TextChannel = await _textChannelCreated.Task;
        VoiceChannel = await _voiceChannelCreated.Task;
        Client.ChannelCreated -= OnChannelCreated;
    }

    private Task OnChannelCreated(SocketChannel arg)
    {
        if (arg is SocketVoiceChannel { Name: VoiceChannelName } voiceChannel)
            _voiceChannelCreated.SetResult(voiceChannel);
        else if (arg is SocketTextChannel { Name: TextChannelName } textChannel)
            _textChannelCreated.SetResult(textChannel);
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
