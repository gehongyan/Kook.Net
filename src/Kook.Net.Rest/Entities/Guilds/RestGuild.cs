using System.Collections.Immutable;
using System.Diagnostics;
using Kook.Audio;
using RichModel = Kook.API.Rest.RichGuild;
using ExtendedModel = Kook.API.Rest.ExtendedGuild;
using Model = Kook.API.Guild;

namespace Kook.Rest;

/// <summary>
///     Represents a REST-based guild/server.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestGuild : RestEntity<ulong>, IGuild, IUpdateable
{
    #region RestGuild

    private ImmutableDictionary<uint, RestRole> _roles;
    private ImmutableArray<RestRole> _currentUserRoles;

    private ImmutableDictionary<ulong, RestGuildChannel> _channels;
    private ImmutableArray<GuildEmote> _emotes;

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public string Topic { get; private set; }

    /// <inheritdoc />
    public ulong OwnerId { get; private set; }

    /// <inheritdoc />
    public string Icon { get; private set; }

    /// <inheritdoc />
    public string Banner { get; private set; }

    /// <summary>
    ///     Gets the nickname of the current user in this guild.
    /// </summary>
    public string? CurrentUserNickname { get; private set; }

    /// <summary>
    ///     Gets the display name of the current user in this guild.
    /// </summary>
    public string CurrentUserDisplayName { get; private set; }

    /// <summary>
    ///     Gets the roles of the current user in this guild.
    /// </summary>
    public IReadOnlyCollection<RestRole> CurrentUserRoles => _currentUserRoles.ToReadOnlyCollection();

    /// <inheritdoc />
    public NotifyType NotifyType { get; private set; }

    /// <inheritdoc />
    public string Region { get; private set; }

    /// <inheritdoc />
    public bool IsOpenEnabled { get; private set; }

    /// <inheritdoc />
    public uint? OpenId { get; private set; }

    /// <inheritdoc />
    public ulong? DefaultChannelId { get; private set; }

    /// <inheritdoc />
    public ulong? WelcomeChannelId { get; private set; }

    internal bool Available { get; private set; }

    /// <inheritdoc/>
    public int MaxBitrate => GuildHelper.GetMaxBitrate(this);

    /// <inheritdoc/>
    public ulong MaxUploadLimit => GuildHelper.GetUploadLimit(this);

    /// <summary>
    ///     Gets the built-in role containing all users in this guild.
    /// </summary>
    public RestRole EveryoneRole => GetRole(0) ?? new RestRole(Kook, this, 0);

    /// <inheritdoc cref="IGuild.Emotes"/>
    /// <remarks>
    ///     <note type="warning">
    ///         This property may contain no elements if the current guild is fetched
    ///         via <see cref="KookRestClient.GetGuildAsync"/>. In this case, you must
    ///         use <see cref="GetEmoteAsync"/> to retrieve all emotes.
    ///     </note>
    /// </remarks>
    public IReadOnlyCollection<GuildEmote> Emotes => _emotes.ToReadOnlyCollection();

    /// <summary>
    ///     Gets a collection of all roles in this guild.
    /// </summary>
    public IReadOnlyCollection<RestRole> Roles => _roles.ToReadOnlyCollection();

    /// <summary>
    ///     Gets a collection of all text channels in this guild.
    /// </summary>
    /// <returns>
    ///     A read-only collection of message channels found within this guild.
    /// </returns>
    public IReadOnlyCollection<RestTextChannel> TextChannels =>
        Channels.OfType<RestTextChannel>().ToImmutableArray();

    /// <summary>
    ///     Gets a collection of all voice channels in this guild.
    /// </summary>
    /// <returns>
    ///     A read-only collection of voice channels found within this guild.
    /// </returns>
    public IReadOnlyCollection<RestVoiceChannel> VoiceChannels =>
        Channels.OfType<RestVoiceChannel>().ToImmutableArray();

    /// <summary>
    ///     Gets a collection of all stage channels in this guild.
    /// </summary>
    /// <returns>
    ///     A read-only collection of stage channels found within this guild.
    /// </returns>
    /// <summary>
    ///     Gets a collection of all category channels in this guild.
    /// </summary>
    /// <returns>
    ///     A read-only collection of category channels found within this guild.
    /// </returns>
    public IReadOnlyCollection<RestCategoryChannel> CategoryChannels =>
        Channels.OfType<RestCategoryChannel>().ToImmutableArray();

    /// <summary>
    ///     Gets a collection of all channels in this guild.
    /// </summary>
    public IReadOnlyCollection<RestGuildChannel> Channels => _channels.ToReadOnlyCollection();

    /// <summary>
    ///     Gets the features of this guild.
    /// </summary>
    /// <returns>
    ///     An array of objects representing the features of this guild.
    /// </returns>
    /// <remarks>
    ///      <note type="important">
    ///         What this property represents is not well investigated.
    ///     </note>
    /// </remarks>
    public GuildFeatures Features { get; private set; }

    /// <inheritdoc />
    public int BoostSubscriptionCount { get; private set; }

    /// <inheritdoc />
    public int BufferBoostSubscriptionCount { get; private set; }

    /// <inheritdoc />
    public BoostLevel BoostLevel { get; private set; }

    /// <summary>
    ///     TODO: To be documented.
    /// </summary>
    public int Status { get; private set; }

    /// <summary>
    ///     TODO: To be documented.
    /// </summary>
    public string? AutoDeleteTime { get; private set; }

    /// <inheritdoc cref="IGuild.RecommendInfo"/>
    public RecommendInfo? RecommendInfo { get; private set; }

    internal RestGuild(BaseKookClient client, ulong id)
        : base(client, id)
    {
        _roles = ImmutableDictionary<uint, RestRole>.Empty;
        _currentUserRoles = [];
        _channels = ImmutableDictionary<ulong, RestGuildChannel>.Empty;
        _emotes = [];
        Name = string.Empty;
        Topic = string.Empty;
        Icon = string.Empty;
        Banner = string.Empty;
        Region = string.Empty;
        CurrentUserDisplayName = client.CurrentUser?.Username ?? string.Empty;
    }

    internal static RestGuild Create(BaseKookClient kook, RichModel model)
    {
        RestGuild entity = new(kook, model.Id);
        entity.Update(model);
        return entity;
    }

    internal static RestGuild Create(BaseKookClient kook, ExtendedModel model)
    {
        RestGuild entity = new(kook, model.Id);
        entity.Update(model);
        return entity;
    }

    internal static RestGuild Create(BaseKookClient kook, Model model)
    {
        RestGuild entity = new(kook, model.Id);
        entity.Update(model);
        return entity;
    }

    internal void Update(RichModel model)
    {
        Update(model as ExtendedModel);

        Banner = model.Banner;
        if (Kook.CurrentUser is null)
            throw new InvalidOperationException("The current user is not set well via login.");
        CurrentUserNickname = model.CurrentUserNickname == Kook.CurrentUser.Username ? null : model.CurrentUserNickname;
        CurrentUserDisplayName = CurrentUserNickname ?? Kook.CurrentUser.Username;
        if (model.CurrentUserRoles is not null)
            _currentUserRoles = [..model.CurrentUserRoles.Select(GetRole).OfType<RestRole>()];

        if (model.Emojis is { Length: > 0} )
            _emotes = [..model.Emojis.Select(x => x.ToEntity(model.Id))];
    }

    internal void Update(ExtendedModel model)
    {
        Update(model as Model);

        Features = model.Features;
        BoostSubscriptionCount = model.BoostSubscriptionCount;
        BufferBoostSubscriptionCount = model.BufferBoostSubscriptionCount;
        BoostLevel = model.BoostLevel;
        Status = model.Status;
        AutoDeleteTime = model.AutoDeleteTime;
        RecommendInfo = model.RecommendInfo?.ToEntity();
    }

    internal void Update(Model model)
    {
        Name = model.Name;
        Topic = model.Topic;
        OwnerId = model.OwnerId;
        Icon = model.Icon;
        NotifyType = model.NotifyType;
        Region = model.Region;
        IsOpenEnabled = model.EnableOpen;
        OpenId = model.OpenId != 0 ? model.OpenId : null;
        if (model.DefaultChannelIdSetting != 0)
            DefaultChannelId = model.DefaultChannelIdSetting;
        else if (model.DefaultChannelId != 0)
            DefaultChannelId = model.DefaultChannelId;
        else
            DefaultChannelId = null;
        WelcomeChannelId = model.WelcomeChannelId != 0 ? model.WelcomeChannelId : null;
        Available = true;

        if (model.Roles is { Length: 0 })
        {
            ImmutableDictionary<uint, RestRole>.Builder roles =
                ImmutableDictionary.CreateBuilder<uint, RestRole>();
            foreach (API.Role roleModel in model.Roles)
                roles[roleModel.Id] = RestRole.Create(Kook, this, roleModel);
            _roles = roles.ToImmutable();
        }

        if (model.Channels is not null)
        {
            ImmutableDictionary<ulong, RestGuildChannel>.Builder channels =
                ImmutableDictionary.CreateBuilder<ulong, RestGuildChannel>();
            foreach (API.Channel channelModel in model.Channels)
                channels[channelModel.Id] = RestGuildChannel.Create(Kook, this, channelModel);
            _channels = channels.ToImmutable();
        }
    }

    #endregion

    #region Generals

    /// <inheritdoc />
    public async Task UpdateAsync(RequestOptions? options = null)
    {
        ExtendedModel model = await Kook.ApiClient.GetGuildAsync(Id, options).ConfigureAwait(false);
        Update(model);
    }

    /// <inheritdoc />
    public Task LeaveAsync(RequestOptions? options = null) =>
        GuildHelper.LeaveAsync(this, Kook, options);

    /// <inheritdoc />
    public Task<ImmutableDictionary<IUser, IReadOnlyCollection<BoostSubscriptionMetadata>>> GetBoostSubscriptionsAsync(
        RequestOptions? options = null) =>
        GuildHelper.GetBoostSubscriptionsAsync(this, Kook, options);

    /// <inheritdoc />
    public Task<ImmutableDictionary<IUser, IReadOnlyCollection<BoostSubscriptionMetadata>>> GetActiveBoostSubscriptionsAsync(
        RequestOptions? options = null) =>
        GuildHelper.GetActiveBoostSubscriptionsAsync(this, Kook, options);

    #endregion

    #region Bans

    /// <summary>
    ///     Gets a collection of all users banned in this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
    ///     ban objects that this guild currently possesses, with each object containing the user banned and reason
    ///     behind the ban.
    /// </returns>
    public Task<IReadOnlyCollection<RestBan>> GetBansAsync(RequestOptions? options = null) =>
        GuildHelper.GetBansAsync(this, Kook, options);

    /// <summary>
    ///     Gets a ban object for a banned user.
    /// </summary>
    /// <param name="user">The banned user.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a ban object, which
    ///     contains the user information and the reason for the ban; <c>null</c> if the ban entry cannot be found.
    /// </returns>
    public Task<RestBan?> GetBanAsync(IUser user, RequestOptions? options = null) =>
        GuildHelper.GetBanAsync(this, Kook, user.Id, options);

    /// <summary>
    ///     Gets a ban object for a banned user.
    /// </summary>
    /// <param name="userId">The identifier for the banned user.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a ban object, which
    ///     contains the user information and the reason for the ban; <c>null</c> if the ban entry cannot be found.
    /// </returns>
    public Task<RestBan?> GetBanAsync(ulong userId, RequestOptions? options = null) =>
        GuildHelper.GetBanAsync(this, Kook, userId, options);

    /// <inheritdoc />
    public Task AddBanAsync(IUser user, int pruneDays = 0, string? reason = null, RequestOptions? options = null) =>
        GuildHelper.AddBanAsync(this, Kook, user.Id, pruneDays, reason, options);

    /// <inheritdoc />
    public Task AddBanAsync(ulong userId, int pruneDays = 0, string? reason = null, RequestOptions? options = null) =>
        GuildHelper.AddBanAsync(this, Kook, userId, pruneDays, reason, options);

    /// <inheritdoc />
    public Task RemoveBanAsync(IUser user, RequestOptions? options = null) =>
        GuildHelper.RemoveBanAsync(this, Kook, user.Id, options);

    /// <inheritdoc />
    public Task RemoveBanAsync(ulong userId, RequestOptions? options = null) =>
        GuildHelper.RemoveBanAsync(this, Kook, userId, options);

    #endregion

    #region Invites

    /// <inheritdoc cref="M:Kook.IGuild.CreateInviteAsync(Kook.InviteMaxAge,Kook.InviteMaxUses,Kook.RequestOptions)" />
    public async Task<RestInvite> CreateInviteAsync(int? maxAge = 604800,
        int? maxUses = null, RequestOptions? options = null) =>
        await GuildHelper.CreateInviteAsync(this, Kook, maxAge, maxUses, options).ConfigureAwait(false);

    /// <inheritdoc cref="M:Kook.IGuild.CreateInviteAsync(System.Nullable{System.Int32},System.Nullable{System.Int32},Kook.RequestOptions)" />
    public async Task<RestInvite> CreateInviteAsync(InviteMaxAge maxAge = InviteMaxAge._604800,
        InviteMaxUses maxUses = InviteMaxUses.Unlimited, RequestOptions? options = null) =>
        await GuildHelper.CreateInviteAsync(this, Kook, maxAge, maxUses, options).ConfigureAwait(false);

    #endregion

    #region Roles

    /// <summary>
    ///     Gets a role in this guild.
    /// </summary>
    /// <param name="id">The identifier for the role.</param>
    /// <returns>
    ///     A role that is associated with the specified <paramref name="id"/>; <c>null</c> if none is found.
    /// </returns>
    public RestRole? GetRole(uint id) => _roles.TryGetValue(id, out RestRole? value) ? value : null;

    /// <summary>
    ///     Creates a new role with the provided name.
    /// </summary>
    /// <param name="name">The new name for the role.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous creation operation. The task result contains the newly created
    ///     role.
    /// </returns>
    public async Task<RestRole> CreateRoleAsync(string? name = null, RequestOptions? options = null)
    {
        RestRole role = await GuildHelper.CreateRoleAsync(this, Kook, name, options).ConfigureAwait(false);
        _roles = _roles.Add(role.Id, role);
        return role;
    }

    #endregion

    #region Users

    /// <summary>
    ///     Gets a collection of all users in this guild.
    /// </summary>
    /// <remarks>
    ///     This method retrieves all users found within this guild.
    /// </remarks>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a collection of guild
    ///     users found within this guild.
    /// </returns>
    public IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> GetUsersAsync(RequestOptions? options = null) =>
        GuildHelper.GetUsersAsync(this, Kook, KookConfig.MaxUsersPerBatch, 1, options);

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
    ///     associated with the specified <paramref name="id"/>; <c>null</c> if none is found.
    /// </returns>
    public Task<RestGuildUser> GetUserAsync(ulong id, RequestOptions? options = null) =>
        GuildHelper.GetUserAsync(this, Kook, id, options);

    /// <summary>
    ///     Gets the current user for this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the currently logged-in
    ///     user within this guild.
    /// </returns>
    public Task<RestGuildUser> GetCurrentUserAsync(RequestOptions? options = null)
    {
        if (Kook.CurrentUser is null)
            throw new InvalidOperationException("The current user is not set well via login.");
        return GuildHelper.GetUserAsync(this, Kook, Kook.CurrentUser.Id, options);
    }

    /// <summary>
    ///     Gets the owner of this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the owner of this guild.
    /// </returns>
    public Task<RestGuildUser> GetOwnerAsync(RequestOptions? options = null) =>
        GuildHelper.GetUserAsync(this, Kook, OwnerId, options);

    /// <summary>
    ///     Gets a collection of users in this guild that the name or nickname contains the
    ///     provided <c>string</c> at <paramref name="func"/>.
    /// </summary>
    /// <remarks>
    ///     The <paramref name="limit"/> can not be higher than <see cref="KookConfig.MaxUsersPerBatch"/>.
    /// </remarks>
    /// <param name="func">A delegate containing the properties to search users with.</param>
    /// <param name="limit">The maximum number of users to be gotten.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a collection of guild
    ///     users that matches the properties with the provided <see cref="Action{SearchGuildMemberProperties}"/> at <paramref name="func"/>.
    /// </returns>
    public IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> SearchUsersAsync(
        Action<SearchGuildMemberProperties> func, int limit = KookConfig.MaxUsersPerBatch,
        RequestOptions? options = null) =>
        GuildHelper.SearchUsersAsync(this, Kook, func, limit, options);

    #endregion

    #region Channels

    /// <summary>
    ///     Gets a collection of all channels in this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
    ///     generic channels found within this guild.
    /// </returns>
    public Task<IReadOnlyCollection<RestGuildChannel>> GetChannelsAsync(RequestOptions? options = null) =>
        GuildHelper.GetChannelsAsync(this, Kook, options);

    /// <summary>
    ///     Gets a channel in this guild.
    /// </summary>
    /// <param name="id">The identifier for the channel.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the generic channel
    ///     associated with the specified <paramref name="id"/>; <c>null</c> if none is found.
    /// </returns>
    public Task<RestGuildChannel> GetChannelAsync(ulong id, RequestOptions? options = null) =>
        GuildHelper.GetChannelAsync(this, Kook, id, options);

    /// <summary>
    ///     Gets a text channel in this guild.
    /// </summary>
    /// <param name="id">The identifier for the text channel.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the text channel
    ///     associated with the specified <paramref name="id"/>; <c>null</c> if none is found.
    /// </returns>
    public async Task<RestTextChannel?> GetTextChannelAsync(ulong id, RequestOptions? options = null)
    {
        RestGuildChannel channel = await GuildHelper
            .GetChannelAsync(this, Kook, id, options)
            .ConfigureAwait(false);
        return channel as RestTextChannel;
    }

    /// <summary>
    ///     Gets a collection of all text channels in this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
    ///     message channels found within this guild.
    /// </returns>
    public async Task<IReadOnlyCollection<RestTextChannel>> GetTextChannelsAsync(RequestOptions? options = null)
    {
        IReadOnlyCollection<RestGuildChannel> channels = await GuildHelper
            .GetChannelsAsync(this, Kook, options)
            .ConfigureAwait(false);
        return [..channels.OfType<RestTextChannel>()];
    }

    /// <summary>
    ///     Gets a voice channel in this guild.
    /// </summary>
    /// <param name="id">The identifier for the voice channel.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the voice channel associated
    ///     with the specified <paramref name="id"/>; <c>null</c> if none is found.
    /// </returns>
    public async Task<RestVoiceChannel?> GetVoiceChannelAsync(ulong id, RequestOptions? options = null)
    {
        RestGuildChannel channel = await GuildHelper
            .GetChannelAsync(this, Kook, id, options)
            .ConfigureAwait(false);
        return channel as RestVoiceChannel;
    }

    /// <summary>
    ///     Gets a collection of all voice channels in this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
    ///     voice channels found within this guild.
    /// </returns>
    public async Task<IReadOnlyCollection<RestVoiceChannel>> GetVoiceChannelsAsync(RequestOptions? options = null)
    {
        IReadOnlyCollection<RestGuildChannel> channels = await GuildHelper
            .GetChannelsAsync(this, Kook, options)
            .ConfigureAwait(false);
        return [..channels.OfType<RestVoiceChannel>()];
    }

    /// <summary>
    ///     Gets a category channel in this guild.
    /// </summary>
    /// <param name="id">The identifier for the category channel.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the category channel associated
    ///     with the specified <paramref name="id"/>; <c>null</c> if none is found.
    /// </returns>
    public async Task<RestCategoryChannel?> GetCategoryChannelAsync(ulong id, RequestOptions? options = null)
    {
        RestGuildChannel channel = await GuildHelper
            .GetChannelAsync(this, Kook, id, options)
            .ConfigureAwait(false);
        return channel as RestCategoryChannel;
    }

    /// <summary>
    ///     Gets a collection of all category channels in this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
    ///     category channels found within this guild.
    /// </returns>
    public async Task<IReadOnlyCollection<RestCategoryChannel>> GetCategoryChannelsAsync(RequestOptions? options = null)
    {
        IReadOnlyCollection<RestGuildChannel> channels = await GuildHelper
            .GetChannelsAsync(this, Kook, options)
            .ConfigureAwait(false);
        return [..channels.OfType<RestCategoryChannel>()];
    }

    /// <summary>
    ///     Gets the default text channel in this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the default text channel of this guild;
    ///     <c>null</c> if none is found.
    /// </returns>
    public async Task<RestTextChannel?> GetDefaultChannelAsync(RequestOptions? options = null)
    {
        if (!DefaultChannelId.HasValue) return null;
        RestGuildChannel channel = await GuildHelper
            .GetChannelAsync(this, Kook, DefaultChannelId.Value, options)
            .ConfigureAwait(false);
        return channel as RestTextChannel;
    }

    /// <summary>
    ///     Gets the welcome text channel in this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the welcome text channel of this guild;
    ///     <c>null</c> if none is found.
    /// </returns>
    public async Task<RestTextChannel?> GetWelcomeChannelAsync(RequestOptions? options = null)
    {
        if (!WelcomeChannelId.HasValue) return null;
        RestGuildChannel channel = await GuildHelper
            .GetChannelAsync(this, Kook, WelcomeChannelId.Value, options)
            .ConfigureAwait(false);
        return channel as RestTextChannel;
    }

    /// <summary>
    ///     Creates a new text channel in this guild.
    /// </summary>
    /// <param name="name">The new name for the text channel.</param>
    /// <param name="func">The delegate containing the properties to be applied to the channel upon its creation.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous creation operation. The task result contains the newly created
    ///     text channel.
    /// </returns>
    public Task<RestTextChannel> CreateTextChannelAsync(string name,
        Action<CreateTextChannelProperties>? func = null, RequestOptions? options = null) =>
        GuildHelper.CreateTextChannelAsync(this, Kook, name, func, options);

    /// <summary>
    ///     Creates a voice channel with the provided name.
    /// </summary>
    /// <param name="name">The name of the new channel.</param>
    /// <param name="func">The delegate containing the properties to be applied to the channel upon its creation.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name" /> is <c>null</c>.</exception>
    /// <returns>
    ///     The created voice channel.
    /// </returns>
    public Task<RestVoiceChannel> CreateVoiceChannelAsync(string name,
        Action<CreateVoiceChannelProperties>? func = null, RequestOptions? options = null) =>
        GuildHelper.CreateVoiceChannelAsync(this, Kook, name, func, options);

    /// <summary>
    ///     Creates a category channel with the provided name.
    /// </summary>
    /// <param name="name">The name of the new channel.</param>
    /// <param name="func">The delegate containing the properties to be applied to the channel upon its creation.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name" /> is <c>null</c>.</exception>
    /// <returns>
    ///     The created category channel.
    /// </returns>
    public Task<RestCategoryChannel> CreateCategoryChannelAsync(string name,
        Action<CreateCategoryChannelProperties>? func = null, RequestOptions? options = null) =>
        GuildHelper.CreateCategoryChannelAsync(this, Kook, name, func, options);

    #endregion

    #region Voices

    /// <inheritdoc />
    public Task MoveUsersAsync(IEnumerable<IGuildUser> users, IVoiceChannel targetChannel,
        RequestOptions? options = null) =>
        ClientHelper.MoveUsersAsync(Kook, users, targetChannel, options);

    #endregion

    #region Emotes

    /// <inheritdoc />
    public Task<IReadOnlyCollection<GuildEmote>> GetEmotesAsync(RequestOptions? options = null) =>
        GuildHelper.GetEmotesAsync(this, Kook, options);

    /// <inheritdoc />
    public Task<GuildEmote?> GetEmoteAsync(string id, RequestOptions? options = null) =>
        GuildHelper.GetEmoteAsync(this, Kook, id, options);

    /// <inheritdoc />
    public Task<GuildEmote> CreateEmoteAsync(string name, Image image, RequestOptions? options = null) =>
        GuildHelper.CreateEmoteAsync(this, Kook, name, image, options);

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <c>null</c>.</exception>
    public Task ModifyEmoteNameAsync(GuildEmote emote, string name, RequestOptions? options = null) =>
        GuildHelper.ModifyEmoteNameAsync(this, Kook, emote, name, options);

    /// <inheritdoc />
    public Task DeleteEmoteAsync(GuildEmote emote, RequestOptions? options = null) =>
        GuildHelper.DeleteEmoteAsync(this, Kook, emote.Id, options);

    #endregion

    #region Invites

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<IInvite>> GetInvitesAsync(RequestOptions? options = null) =>
        await GuildHelper.GetInvitesAsync(this, Kook, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IInvite> IGuild.CreateInviteAsync(int? maxAge, int? maxUses, RequestOptions? options) =>
        await CreateInviteAsync(maxAge, maxUses, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IInvite> IGuild.CreateInviteAsync(InviteMaxAge maxAge, InviteMaxUses maxUses, RequestOptions? options) =>
        await CreateInviteAsync(maxAge, maxUses, options).ConfigureAwait(false);

    #endregion

    #region IGuild

    /// <inheritdoc />
    IAudioClient? IGuild.AudioClient => null;

    /// <inheritdoc />
    bool IGuild.Available => Available;

    /// <inheritdoc />
    IReadOnlyCollection<IRole> IGuild.Roles => Roles;

    /// <inheritdoc />
    IReadOnlyCollection<GuildEmote> IGuild.Emotes => Emotes;

    /// <inheritdoc />
    IRecommendInfo? IGuild.RecommendInfo => RecommendInfo;

    /// <inheritdoc />
    IRole IGuild.EveryoneRole => EveryoneRole;

    /// <inheritdoc />
    IRole? IGuild.GetRole(uint id) => GetRole(id);

    /// <inheritdoc />
    async Task<IRole> IGuild.CreateRoleAsync(string name, RequestOptions? options) =>
        await CreateRoleAsync(name, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IGuildUser?> IGuild.GetCurrentUserAsync(CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetCurrentUserAsync(options).ConfigureAwait(false)
            : null;

    /// <inheritdoc />
    async Task<IGuildUser?> IGuild.GetOwnerAsync(CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetOwnerAsync(options).ConfigureAwait(false)
            : null;

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IGuildUser>> IGuild.GetUsersAsync(CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? [..await GetUsersAsync(options).FlattenAsync().ConfigureAwait(false)]
            : [];

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">Downloading users is not supported for a REST-based guild.</exception>
    Task IGuild.DownloadUsersAsync(RequestOptions? options) => throw new NotSupportedException();

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">Downloading voice states is not supported for a REST-based guild.</exception>
    Task IGuild.DownloadVoiceStatesAsync(RequestOptions? options) => throw new NotSupportedException();

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">Downloading boost subscriptions is not supported for a REST-based guild.</exception>
    Task IGuild.DownloadBoostSubscriptionsAsync(RequestOptions? options) => throw new NotSupportedException();

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuild.SearchUsersAsync(Action<SearchGuildMemberProperties> func,
        int limit, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? SearchUsersAsync(func, limit, options)
            : AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();

    /// <inheritdoc />
    async Task<IGuildUser?> IGuild.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetUserAsync(id, options).ConfigureAwait(false)
            : null;

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IBan>> IGuild.GetBansAsync(RequestOptions? options) =>
        await GetBansAsync(options).ConfigureAwait(false);

    /// <inheritdoc/>
    async Task<IBan?> IGuild.GetBanAsync(IUser user, RequestOptions? options) =>
        await GetBanAsync(user, options).ConfigureAwait(false);

    /// <inheritdoc/>
    async Task<IBan?> IGuild.GetBanAsync(ulong userId, RequestOptions? options) =>
        await GetBanAsync(userId, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IGuildChannel>> IGuild.GetChannelsAsync(CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetChannelsAsync(options).ConfigureAwait(false)
            : Channels;

    /// <inheritdoc />
    async Task<IGuildChannel?> IGuild.GetChannelAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetChannelAsync(id, options).ConfigureAwait(false)
            : Channels.FirstOrDefault(x => x.Id == id);

    /// <inheritdoc />
    async Task<ITextChannel?> IGuild.GetDefaultChannelAsync(CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetDefaultChannelAsync(options).ConfigureAwait(false)
            : Channels.FirstOrDefault(x => x.Id == DefaultChannelId) as ITextChannel;

    /// <inheritdoc />
    async Task<ITextChannel?> IGuild.GetWelcomeChannelAsync(CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetWelcomeChannelAsync(options).ConfigureAwait(false)
            : Channels.FirstOrDefault(x => x.Id == WelcomeChannelId) as ITextChannel;

    /// <inheritdoc />
    async Task<IReadOnlyCollection<ITextChannel>> IGuild.GetTextChannelsAsync(CacheMode mode,
        RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetTextChannelsAsync(options).ConfigureAwait(false)
            : TextChannels;

    /// <inheritdoc />
    async Task<ITextChannel?> IGuild.GetTextChannelAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetTextChannelAsync(id, options).ConfigureAwait(false)
            : TextChannels.FirstOrDefault(x => x.Id == id);

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IVoiceChannel>> IGuild.GetVoiceChannelsAsync(CacheMode mode,
        RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetVoiceChannelsAsync(options).ConfigureAwait(false)
            : VoiceChannels;

    /// <inheritdoc />
    async Task<IVoiceChannel?> IGuild.GetVoiceChannelAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetVoiceChannelAsync(id, options).ConfigureAwait(false)
            : VoiceChannels.FirstOrDefault(x => x.Id == id);

    /// <inheritdoc />
    async Task<IReadOnlyCollection<ICategoryChannel>> IGuild.GetCategoryChannelsAsync(CacheMode mode,
        RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetCategoryChannelsAsync(options).ConfigureAwait(false)
            : CategoryChannels;

    /// <inheritdoc />
    async Task<ITextChannel> IGuild.CreateTextChannelAsync(string name,
        Action<CreateTextChannelProperties>? func, RequestOptions? options) =>
        await CreateTextChannelAsync(name, func, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IVoiceChannel> IGuild.CreateVoiceChannelAsync(string name,
        Action<CreateVoiceChannelProperties>? func, RequestOptions? options) =>
        await CreateVoiceChannelAsync(name, func, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<ICategoryChannel> IGuild.CreateCategoryChannelAsync(string name,
        Action<CreateCategoryChannelProperties>? func, RequestOptions? options) =>
        await CreateCategoryChannelAsync(name, func, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Stream> GetBadgeAsync(BadgeStyle style = BadgeStyle.GuildName, RequestOptions? options = null) =>
        await GuildHelper.GetBadgeAsync(this, Kook, style, options).ConfigureAwait(false);

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
