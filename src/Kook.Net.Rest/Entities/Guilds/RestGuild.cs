using System.Collections.Immutable;
using System.Diagnostics;
using Kook.Audio;
using RichModel = Kook.API.Rest.RichGuild;
using ExtendedModel = Kook.API.Rest.ExtendedGuild;
using Model = Kook.API.Guild;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的服务器。
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
    ///     获取当前用户在此服务器的昵称。
    /// </summary>
    /// <remarks>
    ///     如果当前用户在此服务器未设置昵称，或所设置的昵称与当前用户的用户名相同，则此属性为 <c>null</c>。
    /// </remarks>
    public string? CurrentUserNickname { get; private set; }

    /// <summary>
    ///     获取当前用户在此服务器的显示名称。
    /// </summary>
    /// <remarks>
    ///     如果当前用户在此服务器内设置了昵称，则此属性为设置的昵称；否则为当前用户的用户名。
    /// </remarks>
    public string CurrentUserDisplayName { get; private set; }

    /// <summary>
    ///     获取当前用户在此服务器所拥有的所有角色。
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

    /// <inheritdoc />
    public bool IsAvailable { get; private set; }

    /// <inheritdoc/>
    public int MaxBitrate => GuildHelper.GetMaxBitrate(this);

    /// <inheritdoc/>
    public ulong MaxUploadLimit => GuildHelper.GetUploadLimit(this);

    /// <inheritdoc cref="Kook.IGuild.EveryoneRole" />
    public RestRole EveryoneRole => GetRole(0) ?? new RestRole(Kook, this, 0);

    /// <inheritdoc cref="Kook.IGuild.Emotes"/>
    /// <remarks>
    ///     <note type="warning">
    ///         如果当前服务器是通过 <see cref="KookRestClient.GetGuildAsync"/> 获取的，此属性可能不包含任何元素。访问
    ///         <see cref="Kook.Rest.RestGuild.GetEmoteAsync(System.String,Kook.RequestOptions)"/>
    ///         以获取所有服务器自定义表情。
    ///     </note>
    /// </remarks>
    public IReadOnlyCollection<GuildEmote> Emotes => _emotes.ToReadOnlyCollection();

    /// <inheritdoc cref="Kook.IGuild.Roles" />
    public IReadOnlyCollection<RestRole> Roles => _roles.ToReadOnlyCollection();

    /// <summary>
    ///     获取此服务器中所有具有文字聊天能力的频道。
    /// </summary>
    /// <remarks>
    ///     语音频道也是一种文字频道，此计算属性本意用于获取所有具有文字聊天能力的频道，通过此方法获取到的文字频道列表中也包含了语音频道。
    ///     如需获取频道的实际类型，请参考 <see cref="Kook.ChannelExtensions.GetChannelType(Kook.IChannel)"/>。
    /// </remarks>
    public IReadOnlyCollection<RestTextChannel> TextChannels => Channels.OfType<RestTextChannel>().ToImmutableArray();

    /// <summary>
    ///     获取此服务器中的所有帖子频道。
    /// </summary>
    public IReadOnlyCollection<RestThreadChannel> ThreadChannels => Channels.OfType<RestThreadChannel>().ToImmutableArray();

    /// <summary>
    ///     获取此服务器中所有具有语音聊天能力的频道。
    /// </summary>
    public IReadOnlyCollection<RestVoiceChannel> VoiceChannels => Channels.OfType<RestVoiceChannel>().ToImmutableArray();

    /// <summary>
    ///     获取此服务器中的所有分组频道。
    /// </summary>
    public IReadOnlyCollection<RestCategoryChannel> CategoryChannels => Channels.OfType<RestCategoryChannel>().ToImmutableArray();

    /// <summary>
    ///     获取此服务器中的所有频道。
    /// </summary>
    public IReadOnlyCollection<RestGuildChannel> Channels => _channels.ToReadOnlyCollection();

    /// <inheritdoc />
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

    /// <inheritdoc cref="Kook.IGuild.RecommendInfo"/>
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
        if (Kook.CurrentUser is null)
            throw new InvalidOperationException("The current user is not set well via login.");
        if (model.UserConfig is { } userConfig)
            CurrentUserNickname = userConfig.Nickname == Kook.CurrentUser.Username ? null : userConfig.Nickname;
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
        IsAvailable = true;

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

    /// <inheritdoc cref="Kook.IGuild.GetBansAsync(Kook.RequestOptions)" />
    public Task<IReadOnlyCollection<RestBan>> GetBansAsync(RequestOptions? options = null) =>
        GuildHelper.GetBansAsync(this, Kook, options);

    /// <inheritdoc cref="Kook.IGuild.GetBanAsync(Kook.IUser,Kook.RequestOptions)" />
    public Task<RestBan?> GetBanAsync(IUser user, RequestOptions? options = null) =>
        GuildHelper.GetBanAsync(this, Kook, user.Id, options);

    /// <inheritdoc cref="Kook.IGuild.GetBanAsync(System.UInt64,Kook.RequestOptions)" />
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

    /// <inheritdoc cref="Kook.IGuild.CreateInviteAsync(Kook.InviteMaxAge,Kook.InviteMaxUses,Kook.RequestOptions)" />
    public async Task<RestInvite> CreateInviteAsync(int? maxAge = 604800,
        int? maxUses = null, RequestOptions? options = null) =>
        await GuildHelper.CreateInviteAsync(this, Kook, maxAge, maxUses, options).ConfigureAwait(false);

    /// <inheritdoc cref="Kook.IGuild.CreateInviteAsync(System.Nullable{System.Int32},System.Nullable{System.Int32},Kook.RequestOptions)" />
    public async Task<RestInvite> CreateInviteAsync(InviteMaxAge maxAge = InviteMaxAge._604800,
        InviteMaxUses maxUses = InviteMaxUses.Unlimited, RequestOptions? options = null) =>
        await GuildHelper.CreateInviteAsync(this, Kook, maxAge, maxUses, options).ConfigureAwait(false);

    #endregion

    #region Roles

    /// <inheritdoc cref="Kook.IGuild.GetRole(System.UInt32)" />
    public RestRole? GetRole(uint id) => _roles.TryGetValue(id, out RestRole? value) ? value : null;

    /// <inheritdoc cref="Kook.IGuild.CreateRoleAsync(System.String,Kook.RequestOptions)" />
    public async Task<RestRole> CreateRoleAsync(string? name = null, RequestOptions? options = null)
    {
        RestRole role = await GuildHelper.CreateRoleAsync(this, Kook, name, options).ConfigureAwait(false);
        _roles = _roles.Add(role.Id, role);
        return role;
    }

    #endregion

    #region Users

    /// <summary>
    ///     获取此服务器内的所有用户。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器内的所有用户。 </returns>
    public IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> GetUsersAsync(RequestOptions? options = null) =>
        GuildHelper.GetUsersAsync(this, Kook, KookConfig.MaxUsersPerBatch, 1, options);

    /// <summary>
    ///     获取此服务器内的用户。
    /// </summary>
    /// <param name="id"> 要获取的用户的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含与指定的 <paramref name="id"/> 关联的用户；如果未找到，则返回 <c>null</c>。 </returns>
    public Task<RestGuildUser> GetUserAsync(ulong id, RequestOptions? options = null) =>
        GuildHelper.GetUserAsync(this, Kook, id, options);

    /// <summary>
    ///     获取此服务器内当前登录的用户。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器内当前登录的用户。 </returns>
    public Task<RestGuildUser> GetCurrentUserAsync(RequestOptions? options = null)
    {
        if (Kook.CurrentUser is null)
            throw new InvalidOperationException("The current user is not set well via login.");
        return GuildHelper.GetUserAsync(this, Kook, Kook.CurrentUser.Id, options);
    }

    /// <summary>
    ///     获取此服务器的所有者。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器的所有者。 </returns>
    public Task<RestGuildUser> GetOwnerAsync(RequestOptions? options = null) =>
        GuildHelper.GetUserAsync(this, Kook, OwnerId, options);

    /// <summary>
    ///     获取此服务器内与指定搜索条件匹配的用户。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取所有与指定搜索条件匹配的用户。此方法会根据 <see cref="Kook.KookConfig.MaxUsersPerBatch"/>
    ///     将请求拆分。换句话说，如果搜索结果有 3000 名用户，而 <see cref="Kook.KookConfig.MaxUsersPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 60 个单独请求，因此异步枚举器会异步枚举返回 60 个响应。
    ///     <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///     方法可以展开这 60 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="func"> 一个包含设置服务器用户搜索条件属性的委托。 </param>
    /// <param name="limit"> 要获取搜索到的服务器用户的数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的服务器用户集合的异步可枚举对象。 </returns>
    public IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> SearchUsersAsync(
        Action<SearchGuildMemberProperties> func, int limit = KookConfig.MaxUsersPerBatch, RequestOptions? options = null) =>
        GuildHelper.SearchUsersAsync(this, Kook, func, limit, options);

    #endregion

    #region Channels

    /// <summary>
    ///     获取此服务器的所有频道。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器的所有频道。 </returns>
    public Task<IReadOnlyCollection<RestGuildChannel>> GetChannelsAsync(RequestOptions? options = null) =>
        GuildHelper.GetChannelsAsync(this, Kook, options);

    /// <summary>
    ///     获取此服务器内的频道。
    /// </summary>
    /// <param name="id"> 要获取的频道的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含与指定的 <paramref name="id"/> 关联的频道；如果未找到，则返回 <c>null</c>。 </returns>
    public Task<RestGuildChannel> GetChannelAsync(ulong id, RequestOptions? options = null) =>
        GuildHelper.GetChannelAsync(this, Kook, id, options);

    /// <summary>
    ///     获取此服务器内指定具有文字聊天能力的频道。
    /// </summary>
    /// <param name="id"> 要获取的频道的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含与指定的 <paramref name="id"/> 关联的频道；如果未找到，则返回 <c>null</c>。 </returns>
    /// <remarks>
    ///     语音频道也是一种文字频道，此方法本意用于获取具有文字聊天能力的频道。如果通过此方法传入的 ID 对应的频道是语音频道，那么也会返回对应的语音频道实体。
    ///     如需获取频道的实际类型，请参考 <see cref="Kook.ChannelExtensions.GetChannelType(Kook.IChannel)"/>。
    /// </remarks>
    public async Task<RestTextChannel?> GetTextChannelAsync(ulong id, RequestOptions? options = null)
    {
        RestGuildChannel channel = await GuildHelper
            .GetChannelAsync(this, Kook, id, options)
            .ConfigureAwait(false);
        return channel as RestTextChannel;
    }

    /// <summary>
    ///     获取此服务器内的指定帖子频道。
    /// </summary>
    /// <param name="id"> 要获取的频道的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含与指定的 <paramref name="id"/> 关联的频道；如果未找到，则返回 <c>null</c>。 </returns>
    public async Task<RestThreadChannel?> GetThreadChannelAsync(ulong id, RequestOptions? options = null)
    {
        RestGuildChannel channel = await GuildHelper
            .GetChannelAsync(this, Kook, id, options)
            .ConfigureAwait(false);
        return channel as RestThreadChannel;
    }

    /// <summary>
    ///     获取此服务器中所有具有文字聊天能力的频道。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器的所有具有文字聊天能力的频道。 </returns>
    /// <remarks>
    ///     语音频道也是一种文字频道，此方法本意用于获取所有具有文字聊天能力的频道，通过此方法获取到的文字频道列表中也包含了语音频道。
    ///     如需获取频道的实际类型，请参考 <see cref="Kook.ChannelExtensions.GetChannelType(Kook.IChannel)"/>。
    /// </remarks>
    public async Task<IReadOnlyCollection<RestTextChannel>> GetTextChannelsAsync(RequestOptions? options = null)
    {
        IReadOnlyCollection<RestGuildChannel> channels = await GuildHelper
            .GetChannelsAsync(this, Kook, options)
            .ConfigureAwait(false);
        return [..channels.OfType<RestTextChannel>()];
    }

    /// <summary>
    ///     获取此服务器中的所有帖子频道。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器的所有帖子频道。 </returns>
    public async Task<IReadOnlyCollection<RestThreadChannel>> GetThreadChannelsAsync(RequestOptions? options = null)
    {
        IReadOnlyCollection<RestGuildChannel> channels = await GuildHelper
            .GetChannelsAsync(this, Kook, options)
            .ConfigureAwait(false);
        return [..channels.OfType<RestThreadChannel>()];
    }

    /// <summary>
    ///     获取此服务器内指定具有语音聊天能力的频道。
    /// </summary>
    /// <param name="id"> 要获取的频道的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含与指定的 <paramref name="id"/> 关联的频道；如果未找到，则返回 <c>null</c>。 </returns>
    public async Task<RestVoiceChannel?> GetVoiceChannelAsync(ulong id, RequestOptions? options = null)
    {
        RestGuildChannel channel = await GuildHelper
            .GetChannelAsync(this, Kook, id, options)
            .ConfigureAwait(false);
        return channel as RestVoiceChannel;
    }

    /// <summary>
    ///     获取此服务器中所有具有语音聊天能力的频道。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器的所有具有语音聊天能力的频道。 </returns>
    public async Task<IReadOnlyCollection<RestVoiceChannel>> GetVoiceChannelsAsync(RequestOptions? options = null)
    {
        IReadOnlyCollection<RestGuildChannel> channels = await GuildHelper
            .GetChannelsAsync(this, Kook, options)
            .ConfigureAwait(false);
        return [..channels.OfType<RestVoiceChannel>()];
    }

    /// <summary>
    ///     获取此服务器内指定的分组频道。
    /// </summary>
    /// <param name="id"> 要获取的频道的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含与指定的 <paramref name="id"/> 关联的频道；如果未找到，则返回 <c>null</c>。 </returns>
    public async Task<RestCategoryChannel?> GetCategoryChannelAsync(ulong id, RequestOptions? options = null)
    {
        RestGuildChannel channel = await GuildHelper
            .GetChannelAsync(this, Kook, id, options)
            .ConfigureAwait(false);
        return channel as RestCategoryChannel;
    }

    /// <summary>
    ///     获取此服务器中的所有分组频道。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器的所有分组频道。 </returns>
    public async Task<IReadOnlyCollection<RestCategoryChannel>> GetCategoryChannelsAsync(RequestOptions? options = null)
    {
        IReadOnlyCollection<RestGuildChannel> channels = await GuildHelper
            .GetChannelsAsync(this, Kook, options)
            .ConfigureAwait(false);
        return [..channels.OfType<RestCategoryChannel>()];
    }

    /// <summary>
    ///     获取此服务器的默认文字频道。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器的默认文字频道；如果未找到，则返回 <c>null</c>。 </returns>
    public async Task<RestTextChannel?> GetDefaultChannelAsync(RequestOptions? options = null)
    {
        if (!DefaultChannelId.HasValue) return null;
        RestGuildChannel channel = await GuildHelper
            .GetChannelAsync(this, Kook, DefaultChannelId.Value, options)
            .ConfigureAwait(false);
        return channel as RestTextChannel;
    }

    /// <summary>
    ///     获取此服务器的欢迎通知频道。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器的欢迎通知频道；如果未找到，则返回 <c>null</c>。 </returns>
    public async Task<RestTextChannel?> GetWelcomeChannelAsync(RequestOptions? options = null)
    {
        if (!WelcomeChannelId.HasValue) return null;
        RestGuildChannel channel = await GuildHelper
            .GetChannelAsync(this, Kook, WelcomeChannelId.Value, options)
            .ConfigureAwait(false);
        return channel as RestTextChannel;
    }

    /// <inheritdoc cref="Kook.IGuild.CreateTextChannelAsync(System.String,System.Action{Kook.CreateTextChannelProperties},Kook.RequestOptions)" />
    public Task<RestTextChannel> CreateTextChannelAsync(string name,
        Action<CreateTextChannelProperties>? func = null, RequestOptions? options = null) =>
        GuildHelper.CreateTextChannelAsync(this, Kook, name, func, options);

    /// <inheritdoc cref="Kook.IGuild.CreateVoiceChannelAsync(System.String,System.Action{Kook.CreateVoiceChannelProperties},Kook.RequestOptions)" />
    public Task<RestVoiceChannel> CreateVoiceChannelAsync(string name,
        Action<CreateVoiceChannelProperties>? func = null, RequestOptions? options = null) =>
        GuildHelper.CreateVoiceChannelAsync(this, Kook, name, func, options);

    /// <inheritdoc cref="Kook.IGuild.CreateThreadChannelAsync(System.String,System.Action{Kook.CreateThreadChannelProperties},Kook.RequestOptions)" />
    public Task<RestThreadChannel> CreateThreadChannelAsync(string name,
        Action<CreateThreadChannelProperties>? func = null, RequestOptions? options = null) =>
        GuildHelper.CreateThreadChannelAsync(this, Kook, name, func, options);

    /// <inheritdoc cref="Kook.IGuild.CreateCategoryChannelAsync(System.String,System.Action{Kook.CreateCategoryChannelProperties},Kook.RequestOptions)" />
    public Task<RestCategoryChannel> CreateCategoryChannelAsync(string name,
        Action<CreateCategoryChannelProperties>? func = null, RequestOptions? options = null) =>
        GuildHelper.CreateCategoryChannelAsync(this, Kook, name, func, options);

    #endregion

    #region Voices

    /// <inheritdoc />
    public Task MoveUsersAsync(IEnumerable<IGuildUser> users, IVoiceChannel targetChannel,
        RequestOptions? options = null) =>
        ClientHelper.MoveUsersAsync(Kook, users, targetChannel, options);

    /// <inheritdoc />
    public Task DisconnectUserAsync(IGuildUser user, IVoiceChannel? channel, RequestOptions? options = null) =>
        user.DisconnectAsync(channel, options);

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

    IReadOnlyDictionary<ulong, IAudioClient> IGuild.AudioClients => ImmutableDictionary<ulong, IAudioClient>.Empty;

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
    /// <exception cref="NotSupportedException"> 不支持在基于 REST 的服务器上下载用户。 </exception>
    Task IGuild.DownloadUsersAsync(RequestOptions? options) => throw new NotSupportedException();

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> 不支持在基于 REST 的服务器上下载语音状态。 </exception>
    Task IGuild.DownloadVoiceStatesAsync(RequestOptions? options) => throw new NotSupportedException();

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> 不支持在基于 REST 的服务器上下载服务器助力西南西。 </exception>
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
    async Task<IReadOnlyCollection<IThreadChannel>> IGuild.GetThreadChannelsAsync(CacheMode mode,
        RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetThreadChannelsAsync(options).ConfigureAwait(false)
            : ThreadChannels;

    /// <inheritdoc />
    async Task<ITextChannel?> IGuild.GetTextChannelAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetTextChannelAsync(id, options).ConfigureAwait(false)
            : TextChannels.FirstOrDefault(x => x.Id == id);

    /// <inheritdoc />
    async Task<IThreadChannel?> IGuild.GetThreadChannelAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetThreadChannelAsync(id, options).ConfigureAwait(false)
            : ThreadChannels.FirstOrDefault(x => x.Id == id);

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
    async Task<IThreadChannel> IGuild.CreateThreadChannelAsync(string name,
        Action<CreateThreadChannelProperties>? func, RequestOptions? options) =>
        await CreateThreadChannelAsync(name, func, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<ICategoryChannel> IGuild.CreateCategoryChannelAsync(string name,
        Action<CreateCategoryChannelProperties>? func, RequestOptions? options) =>
        await CreateCategoryChannelAsync(name, func, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Stream> GetBadgeAsync(BadgeStyle style = BadgeStyle.GuildName, RequestOptions? options = null) =>
        await GuildHelper.GetBadgeAsync(this, Kook, style, options).ConfigureAwait(false);

    #endregion

    /// <inheritdoc cref="Kook.Rest.RestGuild.Name" />
    /// <returns> 此服务器的名称。 </returns>
    public override string ToString() => Name;

    private string DebuggerDisplay => $"{Name} ({Id})";
}
