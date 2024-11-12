using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kook;

internal sealed class MockedDMChannel : IDMChannel
{
    ulong IEntity<ulong>.Id => throw new NotImplementedException();

    /// <inheritdoc />
    public Guid ChatCode => throw new NotImplementedException();

    /// <inheritdoc />
    public IUser Recipient => throw new NotImplementedException();

    /// <inheritdoc />
    public Task CloseAsync(RequestOptions? options = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(
        string path, string? filename = null, AttachmentType type = AttachmentType.File,
        IQuote? quote = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(
        Stream stream, string filename, AttachmentType type = AttachmentType.File,
        IQuote? quote = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(
        FileAttachment attachment, IQuote? quote = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendTextAsync(
        string text, IQuote? quote = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendTextAsync<T>(int templateId, T parameters, IQuote? quote = null,
        JsonSerializerOptions? jsonSerializerOptions = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendCardAsync(
        ICard card, IQuote? quote = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(
        IEnumerable<ICard> cards, IQuote? quote = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendCardsAsync<T>(int templateId, T parameters, IQuote? quote = null,
        JsonSerializerOptions? jsonSerializerOptions = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    Guid IEntity<Guid>.Id => throw new NotImplementedException();

    Guid IDMChannel.Id => throw new NotImplementedException();

    /// <inheritdoc />
    public string Name => throw new NotImplementedException();

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<IUser>> GetUsersAsync(
        CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<IUser?> GetUserAsync(ulong id,
        CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(
        string path, string? filename = null, AttachmentType type = AttachmentType.File,
        IQuote? quote = null, IUser? ephemeralUser = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(
        Stream stream, string filename, AttachmentType type = AttachmentType.File, IQuote? quote = null,
        IUser? ephemeralUser = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(
        FileAttachment attachment, IQuote? quote = null,
        IUser? ephemeralUser = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendTextAsync(
        string text, IQuote? quote = null, IUser? ephemeralUser = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendTextAsync<T>(int templateId, T parameters, IQuote? quote = null, IUser? ephemeralUser = null,
        JsonSerializerOptions? jsonSerializerOptions = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendCardAsync(
        ICard card, IQuote? quote = null, IUser? ephemeralUser = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(
        IEnumerable<ICard> cards, IQuote? quote = null, IUser? ephemeralUser = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendCardsAsync<T>(int templateId, T parameters, IQuote? quote = null, IUser? ephemeralUser = null,
        JsonSerializerOptions? jsonSerializerOptions = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<IMessage?> GetMessageAsync(Guid id,
        CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(
        int limit = KookConfig.MaxMessagesPerBatch, CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(
        Guid referenceMessageId, Direction dir, int limit = KookConfig.MaxMessagesPerBatch,
        CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(
        IMessage referenceMessage, Direction dir, int limit = KookConfig.MaxMessagesPerBatch,
        CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task DeleteMessageAsync(Guid messageId, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task DeleteMessageAsync(IMessage message, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task ModifyMessageAsync(Guid messageId, Action<MessageProperties> func, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public IReadOnlyCollection<IUser> Recipients => throw new NotImplementedException();
}
