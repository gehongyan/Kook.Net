using Model = KaiHeiLa.API.Channel;

namespace KaiHeiLa.Rest;

/// <summary>
///     Represents a generic REST-based channel.
/// </summary>
public class RestChannel : RestEntity<ulong>, IChannel, IReloadable
{
    #region RestChannel
    
    internal RestChannel(BaseKaiHeiLaClient kaiHeiLa, ulong id)
        : base(kaiHeiLa, id)
    {
    }
    internal static RestChannel Create(BaseKaiHeiLaClient kaiHeiLa, Model model)
    {
        return model.Type switch
        {
            ChannelType.Text or ChannelType.Voice
                => RestGuildChannel.Create(kaiHeiLa, new RestGuild(kaiHeiLa, model.GuildId), model),
            ChannelType.Category => RestCategoryChannel.Create(kaiHeiLa, new RestGuild(kaiHeiLa, model.GuildId), model),
            _ => new RestChannel(kaiHeiLa, model.Id),
        };
    }
    internal static RestChannel Create(BaseKaiHeiLaClient kaiHeiLa, Model model, IGuild guild)
    {
        return model.Type switch
        {
            ChannelType.Text or ChannelType.Voice
                => RestGuildChannel.Create(kaiHeiLa, guild, model),
            ChannelType.Category => RestCategoryChannel.Create(kaiHeiLa, guild, model),
            _ => new RestChannel(kaiHeiLa, model.Id),
        };
    }
    internal virtual void Update(Model model) { }
    
    /// <inheritdoc />
    public virtual Task ReloadAsync(RequestOptions options = null) => Task.Delay(0);

    #endregion

    #region IChannel

    /// <inheritdoc />
    string IChannel.Name => null;

    /// <inheritdoc />
    Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IUser>(null); //Overridden
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
        => AsyncEnumerable.Empty<IReadOnlyCollection<IUser>>(); //Overridden
    
    #endregion
}