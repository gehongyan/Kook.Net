using Kook.Rest;
using System.Collections.Immutable;
using System.Diagnostics;
using Model = Kook.API.Channel;

namespace Kook.WebSocket;

/// <summary>
///     Represents a WebSocket-based category channel.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class SocketCategoryChannel : SocketGuildChannel, ICategoryChannel
{
    #region SocketCategoryChannel

    /// <inheritdoc />
    public override IReadOnlyCollection<SocketGuildUser> Users
        => Guild.Users.Where(x => Permissions.GetValue(
            Permissions.ResolveChannel(Guild, x, this, Permissions.ResolveGuild(Guild, x)),
            ChannelPermission.ViewChannel)).ToImmutableArray();

    /// <summary>
    ///     Gets the child channels of this category.
    /// </summary>
    /// <returns>
    ///     A read-only collection of <see cref="SocketGuildChannel" /> whose
    ///     <see cref="Kook.INestedChannel.CategoryId" /> matches the identifier of this category channel.
    /// </returns>
    public IReadOnlyCollection<SocketGuildChannel> Channels
        => Guild.Channels.Where(x => x is INestedChannel nestedChannel && nestedChannel.CategoryId == Id).ToImmutableArray();

    internal SocketCategoryChannel(KookSocketClient kook, ulong id, SocketGuild guild)
        : base(kook, id, guild)
    {
        Type = ChannelType.Category;
    }
    internal new static SocketCategoryChannel Create(SocketGuild guild, ClientState state, Model model)
    {
        var entity = new SocketCategoryChannel(guild.Kook, model.Id, guild);
        entity.Update(state, model);
        return entity;
    }

    #endregion

    #region Users
    /// <inheritdoc />
    public override SocketGuildUser GetUser(ulong id)
    {
        var user = Guild.GetUser(id);
        if (user != null)
        {
            var guildPerms = Permissions.ResolveGuild(Guild, user);
            var channelPerms = Permissions.ResolveChannel(Guild, user, this, guildPerms);
            if (Permissions.GetValue(channelPerms, ChannelPermission.ViewChannel))
                return user;
        }
        return null;
    }

    #endregion

    private string DebuggerDisplay => $"{Name} ({Id}, Category)";
    internal new SocketCategoryChannel Clone() => MemberwiseClone() as SocketCategoryChannel;

    #region IGuildChannel

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode,
        RequestOptions options)
    {
        return mode == CacheMode.AllowDownload
            ? ChannelHelper.GetUsersAsync(this, Guild, Kook, KookConfig.MaxUsersPerBatch, 1, options)
            : ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable();
    }
    /// <inheritdoc />
    async Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
    {
        var user = GetUser(id);
        if (user is not null || mode == CacheMode.CacheOnly)
            return user;

        return await ChannelHelper.GetUserAsync(this, Guild, Kook, id, options).ConfigureAwait(false);
    }

    #endregion

    #region IChannel

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
    {
        return mode == CacheMode.AllowDownload
            ? ChannelHelper.GetUsersAsync(this, Guild, Kook, KookConfig.MaxUsersPerBatch, 1, options)
            : ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable();
    }
    /// <inheritdoc />
    async Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
    {
        var user = GetUser(id);
        if (user is not null || mode == CacheMode.CacheOnly)
            return user;

        return await ChannelHelper.GetUserAsync(this, Guild, Kook, id, options).ConfigureAwait(false);
    }

    #endregion

}
