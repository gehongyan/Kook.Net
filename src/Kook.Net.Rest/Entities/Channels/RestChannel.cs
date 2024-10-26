using Model = Kook.API.Channel;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的频道。
/// </summary>
public class RestChannel : RestEntity<ulong>, IChannel, IUpdateable
{
    #region RestChannel

    internal RestChannel(BaseKookClient kook, ulong id)
        : base(kook, id)
    {
    }

    internal static RestChannel Create(BaseKookClient kook, Model model) =>
        model.Type switch
        {
            ChannelType.Text or ChannelType.Voice or ChannelType.Thread => RestGuildChannel.Create(kook, new RestGuild(kook, model.GuildId), model),
            ChannelType.Category => RestCategoryChannel.Create(kook, new RestGuild(kook, model.GuildId), model),
            _ => new RestChannel(kook, model.Id)
        };

    internal static RestChannel Create(BaseKookClient kook, Model model, IGuild guild) =>
        model.Type switch
        {
            ChannelType.Text or ChannelType.Voice or ChannelType.Thread => RestGuildChannel.Create(kook, guild, model),
            ChannelType.Category => RestCategoryChannel.Create(kook, guild, model),
            _ => new RestChannel(kook, model.Id)
        };

    internal virtual void Update(Model model)
    {
    }

    /// <inheritdoc />
    public virtual Task UpdateAsync(RequestOptions? options = null) => Task.CompletedTask;

    #endregion

    #region IChannel

    /// <inheritdoc />
    string IChannel.Name => string.Empty;

    /// <inheritdoc />
    Task<IUser?> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IUser?>(null); //Overridden

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions? options) =>
        AsyncEnumerable.Empty<IReadOnlyCollection<IUser>>(); //Overridden

    #endregion
}
