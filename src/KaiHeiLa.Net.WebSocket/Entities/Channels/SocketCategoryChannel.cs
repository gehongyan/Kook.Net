using System.Collections.Immutable;
using System.Diagnostics;
using KaiHeiLa.Rest;
using Model = KaiHeiLa.API.Channel;

namespace KaiHeiLa.WebSocket;

/// <summary>
///     Represents a WebSocket-based category channel.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class SocketCategoryChannel : SocketGuildChannel, ICategoryChannel
{
    #region SocketCategoryChannel

    internal SocketCategoryChannel(KaiHeiLaSocketClient kaiHeiLa, ulong id, SocketGuild guild)
        : base(kaiHeiLa, id, guild)
    {
        Type = ChannelType.Category;
    }
    internal new static SocketCategoryChannel Create(SocketGuild guild, ClientState state, Model model)
    {
        var entity = new SocketCategoryChannel(guild.KaiHeiLa, model.Id, guild);
        entity.Update(state, model);
        return entity;
    }
    
    #endregion
    
    private string DebuggerDisplay => $"{Name} ({Id}, Category)";
    
    #region IGuildChannel

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode,
        RequestOptions options)
    {
        return mode == CacheMode.AllowDownload
            ? ChannelHelper.GetUsersAsync(this, Guild, KaiHeiLa, KaiHeiLaConfig.MaxUsersPerBatch, 1, options)
            : ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable();
    }
    /// <inheritdoc />
    async Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
    {
        var user = GetUser(id);
        if (user is not null || mode == CacheMode.CacheOnly)
            return user;

        return await ChannelHelper.GetUserAsync(this, Guild, KaiHeiLa, id, options).ConfigureAwait(false);
    }
    
    #endregion
    
    #region IChannel

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
    {
        return mode == CacheMode.AllowDownload
            ? ChannelHelper.GetUsersAsync(this, Guild, KaiHeiLa, KaiHeiLaConfig.MaxUsersPerBatch, 1, options)
            : ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable();
    }
    /// <inheritdoc />
    async Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
    {
        var user = GetUser(id);
        if (user is not null || mode == CacheMode.CacheOnly)
            return user;

        return await ChannelHelper.GetUserAsync(this, Guild, KaiHeiLa, id, options).ConfigureAwait(false);
    }
    
    #endregion
    
}