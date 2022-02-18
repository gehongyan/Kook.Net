using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using KaiHeiLa.API;
using Model = KaiHeiLa.API.Guild;

namespace KaiHeiLa.Rest;

/// <summary>
///     Represents a REST-based guild/server.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class RestGuild : RestEntity<ulong>, IGuild, IUpdateable
{
    #region RestGuild

    private ImmutableDictionary<uint, RestRole> _roles;
    private ImmutableDictionary<ulong, RestChannel> _channels;
    private ImmutableArray<GuildEmote> _emotes;
    
    /// <inheritdoc />
    public string Name { get; private set; }
    /// <inheritdoc />
    public string Topic { get; private set; }
    /// <inheritdoc />
    public uint MasterId { get; private set; }
    /// <inheritdoc />
    public string Icon { get; private set; }
    /// <inheritdoc />
    public NotifyType NotifyType { get; private set; }
    /// <inheritdoc />
    public string Region { get; private set; }
    /// <inheritdoc />
    public bool IsOpenEnabled { get; private set; }
    /// <inheritdoc />
    public uint OpenId { get; private set; }
    /// <inheritdoc />
    public ulong DefaultChannelId { get; private set; }
    /// <inheritdoc />
    public ulong WelcomeChannelId { get; private set; }

    internal bool Available { get; private set; }
    
    /// <summary>
    ///     Gets the built-in role containing all users in this guild.
    /// </summary>
    public RestRole EveryoneRole => GetRole(0);
    
    /// <summary>
    ///     Gets a collection of all roles in this guild.
    /// </summary>
    public IReadOnlyCollection<RestRole> Roles => _roles.ToReadOnlyCollection();
    
    internal RestGuild(BaseKaiHeiLaClient client, ulong id)
        : base(client, id)
    {
    }
    internal static RestGuild Create(BaseKaiHeiLaClient kaiHeiLa, Model model)
    {
        var entity = new RestGuild(kaiHeiLa, model.Id);
        entity.Update(model);
        return entity;
    }

    internal void Update(Model model)
    {
        Name = model.Name;
        Topic = model.Topic;
        MasterId = model.MasterId;
        Icon = model.Icon;
        NotifyType = model.NotifyType;
        Region = model.Region;
        IsOpenEnabled = model.EnableOpen;
        OpenId = model.OpenId;
        DefaultChannelId = model.DefaultChannelId;
        WelcomeChannelId = model.WelcomeChannelId;

        Available = true;

        var roles = ImmutableDictionary.CreateBuilder<uint, RestRole>();
        if (model.Roles != null)
        {
            for (int i = 0; i < model.Roles.Length; i++)
                roles[model.Roles[i].Id] = RestRole.Create(KaiHeiLa, this, model.Roles[i]);
        }
        _roles = roles.ToImmutable();
        
        var channels = ImmutableDictionary.CreateBuilder<ulong, RestChannel>();
        if (model.Channels != null)
        {
            for (int i = 0; i < model.Channels.Length; i++)
                channels[model.Channels[i].Id] = RestChannel.Create(KaiHeiLa, model.Channels[i], this);
        }
        _channels = channels.ToImmutable();
    }
    
    #endregion

    #region Generals

    /// <inheritdoc />
    public async Task UpdateAsync(RequestOptions options = null)
        => Update(await KaiHeiLa.ApiClient.GetGuildAsync(Id, options).ConfigureAwait(false));

    #endregion
    
    #region Roles

    /// <summary>
    ///     Gets a role in this guild.
    /// </summary>
    /// <param name="id">The identifier for the role.</param>
    /// <returns>
    ///     A role that is associated with the specified <paramref name="id"/>; <see langword="null"/> if none is found.
    /// </returns>
    public RestRole GetRole(uint id)
    {
        if (_roles.TryGetValue(id, out RestRole value))
            return value;
        return null;
    }

    #endregion

    #region IGuild
    /// <inheritdoc />
    bool IGuild.Available => Available;
    
    /// <inheritdoc />
    IRole IGuild.GetRole(uint id)
        => GetRole(id);
    
    /// <inheritdoc />
    IRole IGuild.EveryoneRole => EveryoneRole;

    /// <inheritdoc />
    async Task<IGuildUser> IGuild.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetUserAsync(id, options).ConfigureAwait(false);
        else
            return null;
    }
    
    #endregion

    #region Users
    
    /// <summary>
    ///     Gets a user from this guild.
    /// </summary>
    /// <remarks>
    ///     This method retrieves a user found within this guild.
    /// </remarks>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the guild user
    ///     associated with the specified <paramref name="id"/>; <see langword="null"/> if none is found.
    /// </returns>
    public Task<RestGuildUser> GetUserAsync(ulong id, RequestOptions options = null)
        => GuildHelper.GetUserAsync(this, KaiHeiLa, id, options);

    #endregion
    
    /// <summary>
    ///     Returns the name of the guild.
    /// </summary>
    /// <returns>
    ///     The name of the guild.
    /// </returns>
    public override string ToString() => Name;
    private string DebuggerDisplay => $"{Name} ({Id})";
}