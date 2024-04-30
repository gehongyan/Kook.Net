using System;
using System.Threading.Tasks;
using Kook.WebSocket;
using Xunit;
using Xunit.Abstractions;

namespace Kook;

[CollectionDefinition(nameof(MessageTests), DisableParallelization = true)]
[Trait("Category", "Integration.Socket")]
public class MessageTests : IClassFixture<SocketChannelFixture>, IAsyncDisposable
{
    private readonly ITestOutputHelper _output;
    private readonly KookSocketClient _client;
    private readonly SocketTextChannel _textChannel;

    public MessageTests(SocketChannelFixture channelFixture, ITestOutputHelper output)
    {
        _output = output;
        _textChannel = channelFixture.TextChannel;
        _client = channelFixture.Client;
        _client.Log += LogAsync;
    }

    private Task LogAsync(LogMessage message)
    {
        _output.WriteLine(message.ToString());
        return Task.CompletedTask;
    }

    [Fact]
    public async Task CacheableShouldWork()
    {
        const string content = "CACHEABLE SHOULD WORK";
        Cacheable<IUserMessage, Guid> message = await _textChannel.SendTextAsync(content);
        Assert.False(message.HasValue);
        Assert.Null(message.Value);
        IUserMessage? downloaded = await message.GetOrDownloadAsync();
        Assert.NotNull(downloaded);
        Assert.Equal(content, downloaded.Content);
    }

    [Fact]
    public async Task SendTextAsync()
    {
        const string content = "TEXT CONTENT";
        TaskCompletionSource<SocketMessage> socketMessagePromise = new();
        _client.MessageReceived += ClientOnMessageReceived;
        Cacheable<IUserMessage, Guid> message = await _textChannel.SendTextAsync(content);
        Guid messageId = message.Id;
        SocketMessage socketMessage = await socketMessagePromise.Task;
        Assert.Equal(messageId, socketMessage.Id);
        Assert.Equal(content, socketMessage.Content);
        _client.MessageReceived -= ClientOnMessageReceived;
        return;

        Task ClientOnMessageReceived(SocketMessage message, SocketGuildUser author, SocketTextChannel channel)
        {
            socketMessagePromise.SetResult(message);
            return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        _client.Log -= LogAsync;
    }
}
