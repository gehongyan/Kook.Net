using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Kook
{
    internal sealed class MockedDMChannel : IDMChannel
    {
        ulong IEntity<ulong>.Id => throw new NotImplementedException();

        public Guid ChatCode => throw new NotImplementedException();

        public IUser Recipient => throw new NotImplementedException();

        public Task CloseAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendTextMessageAsync(string text, IQuote quote = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(string path, string fileName = null, IQuote quote = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(Stream stream, string fileName = null, IQuote quote = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(Uri uri, IQuote quote = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendVideoMessageAsync(string path, string fileName = null, IQuote quote = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendVideoMessageAsync(Stream stream, string fileName = null, IQuote quote = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendVideoMessageAsync(Uri uri, IQuote quote = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileMessageAsync(string path, string fileName = null, IQuote quote = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileMessageAsync(Stream stream, string fileName = null, IQuote quote = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileMessageAsync(Uri uri, IQuote quote = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendKMarkdownMessageAsync(string text, IQuote quote = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendCardMessageAsync(IEnumerable<ICard> cards, IQuote quote = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public string Name => throw new NotImplementedException();

        public IAsyncEnumerable<IReadOnlyCollection<IUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IMessage> GetMessageAsync(Guid id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = KookConfig.MaxMessagesPerBatch,
            CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(Guid referenceMessageId, Direction dir,
            int limit = KookConfig.MaxMessagesPerBatch, CacheMode mode = CacheMode.AllowDownload,
            RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage referenceMessage, Direction dir,
            int limit = KookConfig.MaxMessagesPerBatch, CacheMode mode = CacheMode.AllowDownload,
            RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMessageAsync(Guid messageId, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMessageAsync(IMessage message, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task ModifyMessageAsync(Guid messageId, Action<MessageProperties> func, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<IUser> Recipients => throw new NotImplementedException();

        Guid IEntity<Guid>.Id => throw new NotImplementedException();

        Guid IDMChannel.Id => throw new NotImplementedException();
    }
}
