using Model = Kook.API.Channel;

namespace Kook.Rest;

/// <summary>
///     Represents a generic REST-based channel.
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
            ChannelType.Text or ChannelType.Voice
                => RestGuildChannel.Create(kook, new RestGuild(kook, model.GuildId), model),
            ChannelType.Category => RestCategoryChannel.Create(kook, new RestGuild(kook, model.GuildId), model),
            _ => new RestChannel(kook, model.Id)
        };

    internal static RestChannel Create(BaseKookClient kook, Model model, IGuild guild) =>
        model.Type switch
        {
            ChannelType.Text or ChannelType.Voice
                => RestGuildChannel.Create(kook, guild, model),
            ChannelType.Category => RestCategoryChannel.Create(kook, guild, model),
            _ => new RestChannel(kook, model.Id)
        };

    internal virtual void Update(Model model)
    {
    }

    /// <inheritdoc />
    public virtual Task UpdateAsync(RequestOptions options = null) => Task.Delay(0);

    #endregion

    #region IChannel

    /// <inheritdoc />
    string IChannel.Name => null;

    /// <inheritdoc />
    Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options = null)
        => Task.FromResult<IUser>(null); //Overridden

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions? options = null)
        => AsyncEnumerable.Empty<IReadOnlyCollection<IUser>>(); //Overridden

    #endregion
}
