using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Kook.Rest;
using Kook.WebSocket;
using Xunit;

namespace Kook;

[TestCaseOrderer(typeof(PriorityOrderer))]
[CollectionDefinition(nameof(MessageTests), DisableParallelization = true)]
[Trait("Category", "Integration.Socket")]
public class MessageTests : IClassFixture<SocketChannelFixture>, IClassFixture<MessageTestFixture>
{
    private readonly MessageTestFixture _messageFixture;
    private readonly ITestOutputHelper _output;
    private readonly KookSocketClient _client;
    private readonly SocketTextChannel _textChannel;

    public MessageTests(SocketChannelFixture channelFixture, MessageTestFixture messageFixture,
        ITestOutputHelper output)
    {
        _messageFixture = messageFixture;
        _output = output;
        _textChannel = channelFixture.TextChannel;
        _client = channelFixture.Client;
        messageFixture.EnsureLogger(_client, LogAsync);
    }

    [Fact]
    [TestPriority(1)]
    public async Task SendTextAsync()
    {
        // Send a text message
        TaskCompletionSource<SocketMessage> socketMessagePromise = new();
        _client.MessageReceived += ClientOnMessageReceived;
        Cacheable<IUserMessage, Guid> cacheableMessage =
            await _textChannel.SendTextAsync(MessageTestFixture.TextMessageContent);

        // The message content received should be the same as the message sent
        Guid messageId = cacheableMessage.Id;
        SocketMessage socketMessage = await socketMessagePromise.Task.WithTimeout();
        SocketUserMessage socketUserMessage = Assert.IsType<SocketUserMessage>(socketMessage);
        Assert.Equal(MessageType.KMarkdown, socketUserMessage.Type);
        Assert.Equal(messageId, socketUserMessage.Id);
        Assert.Equal(MessageTestFixture.TextMessageContent, socketUserMessage.Content);

        // Clean up
        _messageFixture.TextCacheableMessage = cacheableMessage;
        _messageFixture.TextSocketMessage = socketUserMessage;
        _client.MessageReceived -= ClientOnMessageReceived;
        return;

        Task ClientOnMessageReceived(SocketMessage message, SocketGuildUser author, SocketTextChannel channel)
        {
            socketMessagePromise.SetResult(message);
            return Task.CompletedTask;
        }
    }

    [Fact]
    [TestPriority(2)]
    public async Task CacheableShouldWork()
    {
        Assert.NotNull(_messageFixture.TextCacheableMessage);
        Cacheable<IUserMessage, Guid> cacheableMessage = _messageFixture.TextCacheableMessage.Value;
        // The cacheable should have no value
        Assert.False(cacheableMessage.HasValue);

        // The message should be able to be downloaded
        IUserMessage? downloaded = await cacheableMessage.GetOrDownloadAsync();
        Assert.NotNull(downloaded);
        Assert.Equal(MessageTestFixture.TextMessageContent, downloaded.Content);
        SocketTextChannel channel = Assert.IsType<SocketTextChannel>(downloaded.Channel);

        // The message should be able to be cached
        SocketMessage? cachedMessage = channel.GetCachedMessage(downloaded.Id);
        Assert.NotNull(cachedMessage);
        Assert.Equal(downloaded.Id, cachedMessage.Id);
        Assert.Equal(downloaded.Content, cachedMessage.Content);
    }

    [Fact]
    [TestPriority(3)]
    public async Task SendImageAsync()
    {
        // Send an image message
        const string rawUri = "https://img.kaiheila.cn/assets/2021-01/7kr4FkWpLV0ku0ku.jpeg";
        const string filename = "7kr4FkWpLV0ku0ku.jpeg";
        TaskCompletionSource<SocketMessage> socketMessagePromise = new();
        _client.MessageReceived += ClientOnMessageReceived;
        await using Stream imageStream = await new HttpClient().GetStreamAsync(rawUri, TestContext.Current.CancellationToken);
        string assetUri = await _client.Rest.CreateAssetAsync(imageStream, filename);
        using FileAttachment fileAttachment = new(new Uri(assetUri), filename, AttachmentType.Image);
        Cacheable<IUserMessage, Guid> cacheableMessage = await _textChannel.SendFileAsync(fileAttachment);

        // The message content received should be the same as the message sent
        Guid messageId = cacheableMessage.Id;
        SocketMessage socketMessage = await socketMessagePromise.Task.WithTimeout();
        Assert.Equal(MessageType.Image, socketMessage.Type);
        Assert.Equal(messageId, socketMessage.Id);
        Assert.Equal(assetUri, socketMessage.Content);

        // The message should have an attachment
        Assert.Single(socketMessage.Attachments);
        Attachment attachment = socketMessage.Attachments.Single();
        Assert.Equal(AttachmentType.Image, attachment.Type);
        Assert.Equal(assetUri, attachment.Url);
        Assert.Equal(filename, attachment.Filename);

        // Clean up
        _client.MessageReceived -= ClientOnMessageReceived;
        return;

        Task ClientOnMessageReceived(SocketMessage message, SocketGuildUser author, SocketTextChannel channel)
        {
            socketMessagePromise.SetResult(message);
            return Task.CompletedTask;
        }
    }

    [Fact]
    [TestPriority(4)]
    public async Task SendFileAsync()
    {
        // Send a file message
        const string filename = "test.file";
        int fileSize = (int)(10 * Math.Pow(2, 20));
        TaskCompletionSource<SocketMessage> socketMessagePromise = new();
        _client.MessageReceived += ClientOnMessageReceived;
        byte[] buffer = RandomNumberGenerator.GetBytes(fileSize);
        using MemoryStream stream = new(buffer);
        string assetUri = await _client.Rest.CreateAssetAsync(stream, filename);
        using FileAttachment fileAttachment = new(new Uri(assetUri), filename);
        Cacheable<IUserMessage, Guid> cacheableMessage = await _textChannel.SendFileAsync(fileAttachment);

        // The message content received should be the same as the message sent
        Guid messageId = cacheableMessage.Id;
        SocketMessage socketMessage = await socketMessagePromise.Task.WithTimeout();
        Assert.Equal(MessageType.Card, socketMessage.Type); // File messages are converted to cards
        Assert.Equal(messageId, socketMessage.Id);

        // The message should have an attachment
        Assert.Single(socketMessage.Attachments);
        Attachment attachment = socketMessage.Attachments.Single();
        Assert.Equal(AttachmentType.File, attachment.Type);
        Assert.Equal(assetUri, attachment.Url);
        Assert.Equal(filename, attachment.Filename);
        Assert.Equal(fileSize, attachment.Size);

        // Clean up
        _client.MessageReceived -= ClientOnMessageReceived;
        return;

        Task ClientOnMessageReceived(SocketMessage message, SocketGuildUser author, SocketTextChannel channel)
        {
            socketMessagePromise.SetResult(message);
            return Task.CompletedTask;
        }
    }

    [Fact]
    [TestPriority(5)]
    public async Task MessageReferenceShouldWork()
    {
        Assert.NotNull(_messageFixture.TextSocketMessage);
        SocketUserMessage quotedMessage = _messageFixture.TextSocketMessage;
        TaskCompletionSource<SocketUserMessage> newMessagePromise = new();
        _client.MessageReceived += ClientOnMessageReceived;

        // Send the second message with referencing the first message
        const string newContent = "NEW MESSAGE";
        MessageReference reference = new(quotedMessage.Id);
        Cacheable<IUserMessage, Guid> cacheableNewMessage = await _textChannel.SendTextAsync(newContent, reference);
        SocketUserMessage newMessage = await newMessagePromise.Task.WithTimeout();

        // The second message should have a quote to the first message
        Assert.Equal(cacheableNewMessage.Id, newMessage.Id);
        Assert.Equal(newContent, newMessage.Content);
        Assert.NotNull(newMessage.Quote);
        Assert.Equal(quotedMessage.Id, newMessage.Quote.QuotedMessageId);

        // Modify the second message to remove the quote
        TaskCompletionSource<IUserMessage> modificationPromise = new();
        _client.MessageUpdated += ClientOnMessageUpdated;
        await newMessage.ModifyAsync(x =>
        {
            x.Quote = MessageReference.Empty;
        });
        IUserMessage afterModification = await modificationPromise.Task.WithTimeout();
        Assert.Equal(newMessage.Id, afterModification.Id);
        Assert.Equal(newContent, afterModification.Content);
        Assert.Null(afterModification.Quote);

        // Clean up
        _client.MessageReceived -= ClientOnMessageReceived;
        _client.MessageUpdated -= ClientOnMessageUpdated;
        return;

        Task ClientOnMessageReceived(SocketMessage message, SocketGuildUser author, SocketTextChannel channel)
        {
            SocketUserMessage userMessage = Assert.IsType<SocketUserMessage>(message);
            newMessagePromise.SetResult(userMessage);
            return Task.CompletedTask;
        }

        async Task ClientOnMessageUpdated(Cacheable<IMessage, Guid> cacheableBefore,
            Cacheable<IMessage, Guid> cacheableAfter, SocketTextChannel socketTextChannel)
        {
            IMessage? messageValueAfter = await cacheableAfter.GetOrDownloadAsync();
            Assert.NotNull(messageValueAfter);
            IUserMessage userMessageAfter = Assert.IsType<SocketUserMessage>(messageValueAfter);
            modificationPromise.SetResult(userMessageAfter);
        }
    }

    [Fact]
    [TestPriority(6)]
    public async Task ModifyMessageAsync()
    {
        Assert.NotNull(_messageFixture.TextSocketMessage);
        SocketUserMessage message = _messageFixture.TextSocketMessage;
        Guid messageId = message.Id;
        string contentBefore = message.Content;

        // Modify the message
        const string modifiedContent = "MODIFIED CONTENT";
        TaskCompletionSource<IMessage> beforePromise = new();
        TaskCompletionSource<IMessage> afterPromise = new();
        TaskCompletionSource<SocketTextChannel> channelPromise = new();
        _client.MessageUpdated += ClientOnMessageUpdated;
        Assert.NotNull(message);
        await message.ModifyAsync(x =>
        {
            x.Content = modifiedContent;
        });

        // The message content modification should be received
        IMessage messageBefore = await beforePromise.Task.WithTimeout();
        IMessage messageAfter = await afterPromise.Task.WithTimeout();
        SocketTextChannel channel = await channelPromise.Task.WithTimeout();
        Assert.Equal(messageId, messageBefore.Id);
        Assert.Equal(contentBefore, messageBefore.Content);
        Assert.Equal(messageId, messageAfter.Id);
        Assert.Equal(modifiedContent, messageAfter.Content);
        Assert.Null(messageBefore.EditedTimestamp);
        Assert.NotNull(messageAfter.EditedTimestamp);
        Assert.Equal(messageBefore.Channel.Id, messageAfter.Channel.Id);
        Assert.NotSame(messageBefore, messageAfter);
        Assert.Same(message, messageAfter);
        Assert.Same(_textChannel, channel);

        // Clean up
        _client.MessageUpdated -= ClientOnMessageUpdated;
        return;

        async Task ClientOnMessageUpdated(Cacheable<IMessage, Guid> cacheableBefore,
            Cacheable<IMessage, Guid> cacheableAfter, SocketTextChannel socketTextChannel)
        {
            IMessage? messageValueBefore = await cacheableBefore.GetOrDownloadAsync();
            Assert.NotNull(messageValueBefore);
            beforePromise.SetResult(messageValueBefore);
            IMessage? messageValueAfter = await cacheableAfter.GetOrDownloadAsync();
            Assert.NotNull(messageValueAfter);
            afterPromise.SetResult(messageValueAfter);
            channelPromise.SetResult(socketTextChannel);
        }
    }

    [Fact]
    [TestPriority(7)]
    public async Task DeleteMessageAsync()
    {
        Assert.NotNull(_messageFixture.TextSocketMessage);
        SocketUserMessage message = _messageFixture.TextSocketMessage;
        Guid messageId = message.Id;
        string contentBefore = message.Content;

        // Delete the message
        TaskCompletionSource<IMessage> deletedPromise = new();
        TaskCompletionSource<SocketTextChannel> channelPromise = new();
        _client.MessageDeleted += ClientOnMessageDeleted;
        await message.DeleteAsync();

        // The message content modification should be received
        IMessage messageDeleted = await deletedPromise.Task.WithTimeout();
        SocketTextChannel channel = await channelPromise.Task.WithTimeout();
        Assert.Equal(messageId, messageDeleted.Id);
        Assert.Equal(contentBefore, messageDeleted.Content);
        Assert.Same(message, messageDeleted);
        Assert.Equal(messageDeleted.Channel.Id, message.Channel.Id);
        Assert.Same(_textChannel, channel);

        // Clean up
        _messageFixture.TextCacheableMessage = null;
        _messageFixture.TextSocketMessage = null;
        _client.MessageDeleted -= ClientOnMessageDeleted;
        return;

        async Task ClientOnMessageDeleted(Cacheable<IMessage, Guid> cacheableDeleted,
            SocketTextChannel socketTextChannel)
        {
            IMessage? messageValueBefore = await cacheableDeleted.GetOrDownloadAsync();
            Assert.NotNull(messageValueBefore);
            deletedPromise.SetResult(messageValueBefore);
            channelPromise.SetResult(socketTextChannel);
        }
    }

    private Task LogAsync(LogMessage logMessage)
    {
        _output.WriteLine(logMessage.ToString());
        return Task.CompletedTask;
    }
}

public class MessageTestFixture : IAsyncDisposable
{
    private Action? _loggerDisposer;

    private bool _loggerSubscribed;
    public const string TextMessageContent = "TEXT CONTENT";
    public Cacheable<IUserMessage, Guid>? TextCacheableMessage { get; set; }
    public SocketUserMessage? TextSocketMessage { get; set; }

    public void EnsureLogger(KookSocketClient client, Func<LogMessage, Task> logAction)
    {
        if (_loggerSubscribed) return;
        client.Log += logAction;
        _loggerDisposer += () => client.Log -= logAction;
        _loggerSubscribed = true;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (TextSocketMessage is not null)
            await TextSocketMessage.DeleteAsync();
        _loggerDisposer?.Invoke();
    }
}
