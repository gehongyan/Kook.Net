using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Kook.Rest;
using Kook.WebSocket;
using Xunit;
using Xunit.Abstractions;

namespace Kook;

[CollectionDefinition(nameof(MessageTests), DisableParallelization = true)]
[Trait("Category", "Integration.Socket")]
public class MessageTests : IClassFixture<SocketChannelFixture>, IAsyncDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _output;
    private readonly KookSocketClient _client;
    private readonly SocketTextChannel _textChannel;

    public MessageTests(SocketChannelFixture channelFixture, ITestOutputHelper output)
    {
        _httpClient = new HttpClient();
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
        // Send a message
        const string content = "CACHEABLE SHOULD WORK";
        TaskCompletionSource socketMessagePromise = new();
        _client.MessageReceived += ClientOnMessageReceived;
        Cacheable<IUserMessage, Guid> cacheableMessage = await _textChannel.SendTextAsync(content);

        // The cacheable should have no value
        Assert.False(cacheableMessage.HasValue);
        Assert.Null(cacheableMessage.Value);

        // The message should be able to be downloaded
        IUserMessage? downloaded = await cacheableMessage.GetOrDownloadAsync();
        Assert.NotNull(downloaded);
        Assert.Equal(content, downloaded.Content);
        SocketTextChannel channel = Assert.IsType<SocketTextChannel>(downloaded.Channel);

        // The message should be able to be cached
        await socketMessagePromise.Task.WithTimeout();
        SocketMessage? cachedMessage = channel.GetCachedMessage(downloaded.Id);
        Assert.NotNull(cachedMessage);
        Assert.Equal(downloaded.Id, cachedMessage.Id);
        Assert.Equal(downloaded.Content, cachedMessage.Content);

        // Clean up
        _client.MessageReceived -= ClientOnMessageReceived;
        return;

        Task ClientOnMessageReceived(SocketMessage message, SocketGuildUser author, SocketTextChannel socketTextChannel)
        {
            socketMessagePromise.SetResult();
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task SendTextAsync()
    {
        // Send a text message
        const string content = "TEXT CONTENT";
        TaskCompletionSource<SocketMessage> socketMessagePromise = new();
        _client.MessageReceived += ClientOnMessageReceived;
        Cacheable<IUserMessage, Guid> cacheableMessage = await _textChannel.SendTextAsync(content);

        // The message content received should be the same as the message sent
        Guid messageId = cacheableMessage.Id;
        SocketMessage socketMessage = await socketMessagePromise.Task.WithTimeout();
        Assert.Equal(MessageType.KMarkdown, socketMessage.Type);
        Assert.Equal(messageId, socketMessage.Id);
        Assert.Equal(content, socketMessage.Content);

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
    public async Task SendImageAsync()
    {
        // Send an image message
        const string rawUri = "https://img.kaiheila.cn/assets/2021-01/7kr4FkWpLV0ku0ku.jpeg";
        const string filename = "7kr4FkWpLV0ku0ku.jpeg";
        TaskCompletionSource<SocketMessage> socketMessagePromise = new();
        _client.MessageReceived += ClientOnMessageReceived;
        await using Stream imageStream = await _httpClient.GetStreamAsync(rawUri);
        string assetUri = await _client.Rest.CreateAssetAsync(imageStream, filename);
        FileAttachment fileAttachment = new(new Uri(assetUri), filename, AttachmentType.Image);
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
        FileAttachment fileAttachment = new(new Uri(assetUri), filename);
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
    public async Task MessageReferenceShouldWork()
    {
        int messageCount = 0;
        TaskCompletionSource<SocketUserMessage> firstMessagePromise = new();
        TaskCompletionSource<SocketUserMessage> secondMessagePromise = new();
        _client.MessageReceived += ClientOnMessageReceived;

        // Send the first message
        const string contentFirst = "MESSAGE 1";
        Cacheable<IUserMessage, Guid> cacheableFirst = await _textChannel.SendTextAsync(contentFirst);
        SocketUserMessage firstMessage = await firstMessagePromise.Task.WithTimeout();

        // Send the second message with referencing the first message
        const string contentSecond = "MESSAGE 2";
        MessageReference reference = new(cacheableFirst.Id);
        Cacheable<IUserMessage, Guid> cacheableSecond = await _textChannel.SendTextAsync(contentSecond, reference);
        SocketUserMessage secondMessage = await secondMessagePromise.Task.WithTimeout();

        // The second message should have a quote to the first message
        Assert.Equal(cacheableFirst.Id, firstMessage.Id);
        Assert.Equal(contentFirst, firstMessage.Content);
        Assert.Equal(cacheableSecond.Id, secondMessage.Id);
        Assert.Equal(contentSecond, secondMessage.Content);
        Assert.NotNull(secondMessage.Quote);
        Assert.Equal(firstMessage.Id, secondMessage.Quote.QuotedMessageId);

        // Modify the second message to remove the quote
        TaskCompletionSource<IUserMessage> modificationPromise = new();
        _client.MessageUpdated += ClientOnMessageUpdated;
        await secondMessage.ModifyAsync(x =>
        {
            x.Quote = MessageReference.Empty;
        });
        IUserMessage afterModification = await modificationPromise.Task.WithTimeout();
        Assert.Equal(secondMessage.Id, afterModification.Id);
        Assert.Equal(contentSecond, afterModification.Content);
        Assert.Null(afterModification.Quote);

        // Clean up
        _client.MessageReceived -= ClientOnMessageReceived;
        _client.MessageUpdated -= ClientOnMessageUpdated;
        return;

        Task ClientOnMessageReceived(SocketMessage message, SocketGuildUser author, SocketTextChannel channel)
        {
            SocketUserMessage userMessage = Assert.IsType<SocketUserMessage>(message);
            if (messageCount == 0)
                firstMessagePromise.SetResult(userMessage);
            else if (messageCount == 1)
                secondMessagePromise.SetResult(userMessage);
            messageCount++;
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
    public async Task ModifyMessageAsync()
    {
        // Send a text message
        const string content = "TEXT CONTENT";
        const string modifiedContent = "TEXT CONTENT MODIFIED";
        Cacheable<IUserMessage, Guid> cacheableMessage = await _textChannel.SendTextAsync(content);
        Guid messageId = cacheableMessage.Id;

        // Modify the message
        TaskCompletionSource<IMessage> beforePromise = new();
        TaskCompletionSource<IMessage> afterPromise = new();
        TaskCompletionSource<SocketTextChannel> channelPromise = new();
        _client.MessageUpdated += ClientOnMessageUpdated;
        IUserMessage? message = await cacheableMessage.GetOrDownloadAsync();
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
        Assert.Equal(content, messageBefore.Content);
        Assert.Equal(messageId, messageAfter.Id);
        Assert.Equal(modifiedContent, messageAfter.Content);
        Assert.Null(messageBefore.EditedTimestamp);
        Assert.NotNull(messageAfter.EditedTimestamp);
        Assert.Equal(messageBefore.Channel.Id, messageAfter.Channel.Id);
        Assert.NotSame(messageBefore, messageAfter);
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
    public async Task DeleteMessageAsync()
    {
        // Send a text message
        const string content = "TEXT CONTENT";
        Cacheable<IUserMessage, Guid> cacheableMessage = await _textChannel.SendTextAsync(content);
        Guid messageId = cacheableMessage.Id;

        // Delete the message
        TaskCompletionSource<IMessage> deletedPromise = new();
        TaskCompletionSource<SocketTextChannel> channelPromise = new();
        _client.MessageDeleted += ClientOnMessageDeleted;
        IUserMessage? message = await cacheableMessage.GetOrDownloadAsync();
        Assert.NotNull(message);
        await message.DeleteAsync();

        // The message content modification should be received
        IMessage messageDeleted = await deletedPromise.Task.WithTimeout();
        SocketTextChannel channel = await channelPromise.Task.WithTimeout();
        Assert.Equal(messageId, messageDeleted.Id);
        Assert.Equal(content, messageDeleted.Content);
        Assert.Equal(messageDeleted.Channel.Id, message.Channel.Id);
        Assert.Same(_textChannel, channel);

        // Clean up
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

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        _client.Log -= LogAsync;
        return ValueTask.CompletedTask;
    }
}
