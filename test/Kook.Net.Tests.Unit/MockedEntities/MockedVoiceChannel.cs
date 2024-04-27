using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Kook.Audio;

namespace Kook;

internal sealed class MockedVoiceChannel : IVoiceChannel
{
    public ulong Id => throw new NotImplementedException();

    public string Name => throw new NotImplementedException();

    public IGuild Guild => throw new NotImplementedException();

    public ulong GuildId => throw new NotImplementedException();

    public int? Position => throw new NotImplementedException();

    public ChannelType Type => throw new NotImplementedException();

    public IReadOnlyCollection<RolePermissionOverwrite> RolePermissionOverwrites => throw new NotImplementedException();

    public IReadOnlyCollection<UserPermissionOverwrite> UserPermissionOverwrites => throw new NotImplementedException();

    public Task ModifyAsync(Action<ModifyGuildChannelProperties> func, RequestOptions? options = null) =>
        throw new NotImplementedException();

    public OverwritePermissions? GetPermissionOverwrite(IRole role) => throw new NotImplementedException();

    public OverwritePermissions? GetPermissionOverwrite(IUser user) => throw new NotImplementedException();

    public Task RemovePermissionOverwriteAsync(IRole role, RequestOptions? options = null) =>
        throw new NotImplementedException();

    public Task RemovePermissionOverwriteAsync(IGuildUser user, RequestOptions? options = null) =>
        throw new NotImplementedException();

    public Task AddPermissionOverwriteAsync(IRole role, RequestOptions? options = null) =>
        throw new NotImplementedException();

    public Task AddPermissionOverwriteAsync(IGuildUser user, RequestOptions? options = null) =>
        throw new NotImplementedException();

    public Task ModifyPermissionOverwriteAsync(IRole role,
        Func<OverwritePermissions, OverwritePermissions> func, RequestOptions? options = null) =>
        throw new NotImplementedException();

    public Task ModifyPermissionOverwriteAsync(IGuildUser user,
        Func<OverwritePermissions, OverwritePermissions> func, RequestOptions? options = null) =>
        throw new NotImplementedException();

    public IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> GetUsersAsync(
        CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) =>
        throw new NotImplementedException();

    Task<IGuildUser?> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        throw new NotImplementedException();

    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions? options) =>
        GetUsersAsync(mode, options);

    Task<IUser?> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        throw new NotImplementedException();

    public Task DeleteAsync(RequestOptions? options = null) => throw new NotImplementedException();

    public ulong? CategoryId => throw new NotImplementedException();

    public bool? IsPermissionSynced => throw new NotImplementedException();

    public ulong CreatorId => throw new NotImplementedException();

    /// <inheritdoc />
    public Task SyncPermissionsAsync(RequestOptions? options = null) => throw new NotImplementedException();

    public Task<ICategoryChannel?> GetCategoryAsync(
        CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) =>
        throw new NotImplementedException();

    public Task<IUser?> GetCreatorAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) =>
        throw new NotImplementedException();

    public Task<IReadOnlyCollection<IInvite>> GetInvitesAsync(RequestOptions? options = null) =>
        throw new NotImplementedException();

    public Task<IInvite> CreateInviteAsync(InviteMaxAge maxAge = InviteMaxAge._604800,
        InviteMaxUses maxUses = InviteMaxUses.Unlimited, RequestOptions? options = null) =>
        throw new NotImplementedException();

    public Task<IInvite> CreateInviteAsync(int? maxAge = 604800, int? maxUses = null,
        RequestOptions? options = null) =>
        throw new NotImplementedException();

    public string PlainTextMention => throw new NotImplementedException();

    public string KMarkdownMention => throw new NotImplementedException();

    public VoiceQuality? VoiceQuality => throw new NotImplementedException();

    public int UserLimit => throw new NotImplementedException();

    /// <inheritdoc />
    public bool? IsVoiceRegionOverwritten => throw new NotImplementedException();

    /// <inheritdoc />
    public string VoiceRegion => throw new NotImplementedException();

    public string ServerUrl => throw new NotImplementedException();

    public bool HasPassword => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<IAudioClient?> ConnectAsync( /*bool selfDeaf = false, bool selfMute = false, */
        bool external = false, bool disconnect = true) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task DisconnectAsync() => throw new NotImplementedException();

    public Task ModifyAsync(Action<ModifyVoiceChannelProperties> func, RequestOptions? options = null) =>
        throw new NotImplementedException();

    public Task<IReadOnlyCollection<IUser>> GetConnectedUsersAsync(
        CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(string path,
        string? filename = null, AttachmentType type = AttachmentType.File, IQuote? quote = null,
        IUser? ephemeralUser = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(Stream stream,
        string filename, AttachmentType type = AttachmentType.File, IQuote? quote = null,
        IUser? ephemeralUser = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(FileAttachment attachment,
        IQuote? quote = null, IUser? ephemeralUser = null,
        RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendTextAsync(string text,
        IQuote? quote = null, IUser? ephemeralUser = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendCardAsync(ICard card,
        IQuote? quote = null, IUser? ephemeralUser = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(IEnumerable<ICard> cards,
        IQuote? quote = null, IUser? ephemeralUser = null, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<IMessage?> GetMessageAsync(Guid id,
        CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(
        int limit = KookConfig.MaxMessagesPerBatch, CacheMode mode = CacheMode.AllowDownload,
        RequestOptions? options = null) =>
        throw new NotImplementedException();

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
    public string Topic => throw new NotImplementedException();

    /// <inheritdoc />
    public int SlowModeInterval => throw new NotImplementedException();

    /// <inheritdoc />
    public Task ModifyAsync(Action<ModifyTextChannelProperties> func, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<IReadOnlyCollection<IMessage>> GetPinnedMessagesAsync(RequestOptions? options = null) =>
        throw new NotImplementedException();
}
