using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Kook
{
    internal sealed class MockedTextChannel : ITextChannel
    {
        public ulong Id => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public IGuild Guild => throw new NotImplementedException();

        public ulong GuildId => throw new NotImplementedException();

        public int? Position => throw new NotImplementedException();

        public ChannelType Type => throw new NotImplementedException();

        public IReadOnlyCollection<RolePermissionOverwrite> RolePermissionOverwrites => throw new NotImplementedException();

        public IReadOnlyCollection<UserPermissionOverwrite> UserPermissionOverwrites => throw new NotImplementedException();

        public Task ModifyAsync(Action<ModifyGuildChannelProperties> func, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public OverwritePermissions? GetPermissionOverwrite(IRole role)
        {
            throw new NotImplementedException();
        }

        public OverwritePermissions? GetPermissionOverwrite(IUser user)
        {
            throw new NotImplementedException();
        }

        public Task RemovePermissionOverwriteAsync(IRole role, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemovePermissionOverwriteAsync(IGuildUser user, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task AddPermissionOverwriteAsync(IRole role, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task AddPermissionOverwriteAsync(IGuildUser user, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task ModifyPermissionOverwriteAsync(IRole role, Func<OverwritePermissions, OverwritePermissions> func, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task ModifyPermissionOverwriteAsync(IGuildUser user, Func<OverwritePermissions, OverwritePermissions> func, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            throw new NotImplementedException();
        }

        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
        {
            return GetUsersAsync(mode, options);
        }

        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public ulong? CategoryId => throw new NotImplementedException();

        public bool? IsPermissionSynced => throw new NotImplementedException();

        public ulong CreatorId => throw new NotImplementedException();

        public Task<ICategoryChannel> GetCategoryAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task SyncPermissionsAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IUser> GetCreatorAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IInvite>> GetInvitesAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IInvite> CreateInviteAsync(InviteMaxAge maxAge = InviteMaxAge._604800, InviteMaxUses maxUses = InviteMaxUses.Unlimited,
            RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IInvite> CreateInviteAsync(int? maxAge, int? maxUses = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public string PlainTextMention => throw new NotImplementedException();

        public string KMarkdownMention => throw new NotImplementedException();

        public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileAsync(string path, string fileName = null, AttachmentType type = AttachmentType.File, IQuote quote = null,
            IUser ephemeralUser = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileAsync(Stream stream, string fileName, AttachmentType type = AttachmentType.File, IQuote quote = null,
            IUser ephemeralUser = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileAsync(FileAttachment attachment, IQuote quote = null, IUser ephemeralUser = null,
            RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendTextAsync(string text, IQuote quote = null, IUser ephemeralUser = null,
            RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendCardAsync(ICard card, IQuote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendCardsAsync(IEnumerable<ICard> cards, IQuote quote = null, IUser ephemeralUser = null,
            RequestOptions options = null)
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

        public string Topic => throw new NotImplementedException();

        public int SlowModeInterval => throw new NotImplementedException();

        public Task ModifyAsync(Action<ModifyTextChannelProperties> func, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IMessage>> GetPinnedMessagesAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }
    }
}
