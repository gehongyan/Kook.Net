using Model = KaiHeiLa.API.Channel;

namespace KaiHeiLa.Rest;

public class RestChannel : RestEntity<ulong>, IChannel, IUpdateable
{
    #region RestChannel
    
    public ulong CreateUserId { get; set; }
    
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
    public virtual Task UpdateAsync(RequestOptions options = null) => Task.Delay(0);

    #endregion

    #region IChannel

    /// <inheritdoc />
    string IChannel.Name => null;

    /// <inheritdoc />
    Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IUser>(null); //Overridden
    #endregion
}