using System.Collections.Immutable;
using System.Diagnostics;
using Kook.API.Rest;
using Model = Kook.API.Rest.ThreadCategory;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的帖子分区。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestThreadCategory : RestEntity<ulong>, IThreadCategory
{
    private ImmutableArray<RolePermissionOverwrite> _rolePermissionOverwrites = [];
    private ImmutableArray<UserPermissionOverwrite> _userPermissionOverwrites = [];

    /// <inheritdoc />
    public IThreadChannel Channel { get; }

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<RolePermissionOverwrite> RolePermissionOverwrites => _rolePermissionOverwrites;

    /// <inheritdoc />
    public IReadOnlyCollection<UserPermissionOverwrite> UserPermissionOverwrites => _userPermissionOverwrites;

    /// <inheritdoc />
    internal RestThreadCategory(BaseKookClient kook, IThreadChannel channel, ulong id)
        : base(kook, id)
    {
        Channel = channel;
        Name = string.Empty;
    }

    internal static RestThreadCategory Create(BaseKookClient kook, IThreadChannel channel, Model model)
    {
        RestThreadCategory entity = new(kook, channel, model.Id);
        entity.Update(model);
        return entity;
    }

    private void Update(Model model)
    {
        Name = model.Name;
        ImmutableArray<RolePermissionOverwrite>.Builder rolePermissions =
            ImmutableArray.CreateBuilder<RolePermissionOverwrite>();
        ImmutableArray<UserPermissionOverwrite>.Builder userPermissions =
            ImmutableArray.CreateBuilder<UserPermissionOverwrite>();
        if (model.Roles is not null)
        {
            foreach (ThreadCategoryPermissionOverwrite overwrite in model.Roles)
            {
                if (overwrite is { Type: PermissionOverwriteTarget.Role, RoleId: not null })
                {
                    rolePermissions.Add(new RolePermissionOverwrite(
                        overwrite.RoleId.Value,
                        new OverwritePermissions(overwrite.Allow, 0)));
                }
                else if (overwrite is { Type: PermissionOverwriteTarget.User, UserId: not null }
                         && ulong.TryParse(overwrite.UserId, out ulong userId))
                {
                    userPermissions.Add(new UserPermissionOverwrite(
                        userId,
                        new OverwritePermissions(overwrite.Allow, 0)));
                }
            }
        }
        rolePermissions.Add(new RolePermissionOverwrite(
            Channel.Guild.EveryoneRole.Id,
            new OverwritePermissions(model.Allow, model.Deny)));
        _rolePermissionOverwrites = rolePermissions.ToImmutable();
        _userPermissionOverwrites = userPermissions.ToImmutable();
    }

    /// <inheritdoc cref="Kook.IThreadCategory.GetThreadsAsync(System.Int32,Kook.RequestOptions)" />
    public IAsyncEnumerable<IReadOnlyCollection<RestThread>> GetThreadsAsync(
        int limit = KookConfig.MaxThreadsPerBatch, RequestOptions? options = null) =>
        ThreadHelper.GetThreadsAsync(Channel, Kook, null, ThreadSortOrder.Inherited, limit, this, options);

    /// <inheritdoc cref="Kook.IThreadCategory.GetThreadsAsync(System.DateTimeOffset,Kook.ThreadSortOrder,System.Int32,Kook.RequestOptions)" />
    public IAsyncEnumerable<IReadOnlyCollection<RestThread>> GetThreadsAsync(DateTimeOffset referenceTimestamp,
        ThreadSortOrder sortOrder = ThreadSortOrder.CreationTime, int limit = KookConfig.MaxThreadsPerBatch,
        RequestOptions? options = null) =>
        ThreadHelper.GetThreadsAsync(Channel, Kook, referenceTimestamp, sortOrder, limit, this, options);

    /// <inheritdoc cref="Kook.IThreadCategory.GetThreadsAsync(Kook.IThread,Kook.ThreadSortOrder,System.Int32,Kook.RequestOptions)" />
    public IAsyncEnumerable<IReadOnlyCollection<RestThread>> GetThreadsAsync(IThread referenceThread,
        ThreadSortOrder sortOrder = ThreadSortOrder.CreationTime,
        int limit = KookConfig.MaxThreadsPerBatch, RequestOptions? options = null) =>
        ThreadHelper.GetThreadsAsync(Channel, Kook, sortOrder switch
        {
            ThreadSortOrder.LatestActivity => referenceThread.LatestActiveTimestamp,
            ThreadSortOrder.CreationTime => referenceThread.Timestamp,
            ThreadSortOrder.Inherited when Channel.DefaultSortOrder is ThreadSortOrder.LatestActivity => referenceThread.LatestActiveTimestamp,
            ThreadSortOrder.Inherited when Channel.DefaultSortOrder is ThreadSortOrder.CreationTime => referenceThread.Timestamp,
            _ => null
        }, sortOrder, limit, this, options);

    /// <inheritdoc cref="Kook.IThreadCategory.CreateThreadAsync(System.String,System.String,System.String,Kook.ThreadTag[],Kook.RequestOptions)" />
    public async Task<RestThread> CreateThreadAsync(string title, string content, string? cover = null,
        ThreadTag[]? tags = null, RequestOptions? options = null) =>
        await ThreadHelper.CreateThreadAsync(Channel, Kook, title, content, cover, this, tags, options).ConfigureAwait(false);

    /// <inheritdoc cref="Kook.IThreadCategory.CreateThreadAsync(System.String,Kook.ICard,System.String,Kook.ThreadTag[],Kook.RequestOptions)" />
    public async Task<RestThread> CreateThreadAsync(string title, ICard card, string? cover = null,
        ThreadTag[]? tags = null, RequestOptions? options = null) =>
        await ThreadHelper.CreateThreadAsync(Channel, Kook, title, card, cover, this, tags, options).ConfigureAwait(false);

    /// <inheritdoc cref="Kook.IThreadCategory.CreateThreadAsync(System.String,System.Collections.Generic.IEnumerable{Kook.ICard},System.String,Kook.ThreadTag[],Kook.RequestOptions)" />
    public async Task<RestThread> CreateThreadAsync(string title, IEnumerable<ICard> cards, string? cover = null,
        ThreadTag[]? tags = null, RequestOptions? options = null) =>
        await ThreadHelper.CreateThreadAsync(Channel, Kook, title, cards, cover, this, tags, options).ConfigureAwait(false);

    #region IThraedCategory

    IAsyncEnumerable<IReadOnlyCollection<IThread>> IThreadCategory.GetThreadsAsync(int limit, RequestOptions? options) =>
        GetThreadsAsync(limit, options);

    IAsyncEnumerable<IReadOnlyCollection<IThread>> IThreadCategory.GetThreadsAsync(DateTimeOffset referenceTimestamp,
        ThreadSortOrder sortOrder, int limit, RequestOptions? options) =>
        GetThreadsAsync(referenceTimestamp, sortOrder, limit, options);

    IAsyncEnumerable<IReadOnlyCollection<IThread>> IThreadCategory.GetThreadsAsync(IThread referenceThread,
        ThreadSortOrder sortOrder, int limit, RequestOptions? options) =>
        GetThreadsAsync(referenceThread, sortOrder, limit, options);

    /// <inheritdoc />
    async Task<IThread> IThreadCategory.CreateThreadAsync(string title, string content, string? cover,
        ThreadTag[]? tags, RequestOptions? options) =>
        await CreateThreadAsync(title, content, cover, tags, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IThread> IThreadCategory.CreateThreadAsync(string title, ICard card, string? cover,
        ThreadTag[]? tags, RequestOptions? options) =>
        await CreateThreadAsync(title, card, cover, tags, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IThread> IThreadCategory.CreateThreadAsync(string title, IEnumerable<ICard> cards, string? cover,
        ThreadTag[]? tags, RequestOptions? options) =>
        await CreateThreadAsync(title, cards, cover, tags, options).ConfigureAwait(false);

    #endregion
}
