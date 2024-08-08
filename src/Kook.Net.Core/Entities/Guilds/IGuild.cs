using System.Collections.Immutable;
using Kook.Audio;

namespace Kook;

/// <summary>
///     表示一个通用的服务器。
/// </summary>
public interface IGuild : IEntity<ulong>
{
    #region General

    /// <summary>
    ///     获取此服务器的名称。
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     获取此服务器的介绍。
    /// </summary>
    string Topic { get; }

    /// <summary>
    ///     获取此服务器所有者的用户 ID。
    /// </summary>
    ulong OwnerId { get; }

    /// <summary>
    ///     获取此服务器图标的 URL。
    /// </summary>
    string Icon { get; }

    /// <summary>
    ///     获取此服务器横幅图像的 URL。
    /// </summary>
    string Banner { get; }

    /// <summary>
    ///     获取此服务器的默认通知类型。
    /// </summary>
    NotifyType NotifyType { get; }

    /// <summary>
    ///     获取此服务器的默认语音服务器区域。
    /// </summary>
    /// <remarks>
    ///     语音服务器区域是指语音服务器所在的地理位置，各个语音服务器区域由一个唯一的字符串表示。 <br />
    ///     可用语音服务器区域参考列表：
    ///     <list type="table">
    ///         <listheader>
    ///             <term> 区域 ID </term>
    ///             <description> 区域名称 </description>
    ///         </listheader>
    ///         <item>
    ///             <term> <c>chengdu</c> </term>
    ///             <description> 西南(成都) </description>
    ///         </item>
    ///         <item>
    ///             <term> <c>beijing</c> </term>
    ///             <description> 华北(北京) </description>
    ///         </item>
    ///         <item>
    ///             <term> <c>shanghai</c> </term>
    ///             <description> 华东(上海) </description>
    ///         </item>
    ///         <item>
    ///             <term> <c>shenzhen</c> </term>
    ///             <description> 华南(深圳) </description>
    ///         </item>
    ///         <item>
    ///             <term> <c>hk</c> </term>
    ///             <description> 亚太(香港) </description>
    ///         </item>
    ///         <item>
    ///             <term> <c>vnga</c> </term>
    ///             <description> 国际专线(助力专享) </description>
    ///         </item>
    ///     </list>
    ///     此列表仅供参考。要获取最新可用服务器区域列表，可在安装 Kook.Net.Experimental 实验性 API 实现包后，在
    ///     <c>BaseKookClient</c> 上调用 <c>GetVoiceRegionsAsync</c> 方法。
    /// </remarks>
    string Region { get; }

    /// <summary>
    ///     获取在此服务内的语音频道上建立的语音客户端。
    /// </summary>
    /// <seealso cref="P:Kook.IAudioChannel.AudioClient"/>
    [Obsolete("Use AudioClients instead.")]
    IAudioClient? AudioClient { get; }

    /// <summary>
    ///     获取在此服务内的语音频道上建立的所有语音客户端。
    /// </summary>
    /// <seealso cref="P:Kook.IAudioChannel.AudioClient"/>
    IReadOnlyDictionary<ulong, IAudioClient> AudioClients { get; }

    /// <summary>
    ///     获取此服务器是否为公开服务器。
    /// </summary>
    bool IsOpenEnabled { get; }

    /// <summary>
    ///     获取此服务器的公开 ID。
    /// </summary>
    /// <remarks>
    ///     当 <see cref="IsOpenEnabled"/> 为 <c>true</c> 时，此属性应该返回一个有效的公开服务器 ID；
    ///     如果 <see cref="IsOpenEnabled"/> 为 <c>false</c>，则此属性应该返回 <c>null</c>。
    /// </remarks>
    uint? OpenId { get; }

    /// <summary>
    ///     获取默认文字频道的 ID。
    /// </summary>
    ulong? DefaultChannelId { get; }

    /// <summary>
    ///     获取欢迎通知频道的 ID。
    /// </summary>
    ulong? WelcomeChannelId { get; }

    /// <summary>
    ///     确定此服务器实体是否已准备就绪以供用户代码访问。
    /// </summary>
    /// <remarks>
    ///     <note>
    ///         此属性仅对基于网关连接的客户端有意义。
    ///     </note>
    ///     此属性为 <c>true</c> 表示，此服务器实体已完整缓存基础数据，并与网关同步。 <br />
    ///     缓存基础数据包括服务器基本信息、频道、角色、频道权限重写、当前用户在服务器内的昵称。
    /// </remarks>
    bool Available { get; }

    /// <summary>
    ///     获取此服务器中的 <c>@全体成员</c> 全体成员角色。
    /// </summary>
    IRole EveryoneRole { get; }

    /// <summary>
    ///     获取此服务器的所有特性。
    /// </summary>
    GuildFeatures Features { get; }

    /// <summary>
    ///     获取此服务器的服务器助力包的数量。
    /// </summary>
    int BoostSubscriptionCount { get; }

    /// <summary>
    ///     获取此服务器来自拥有 BUFF 会员的用的的服务器助力包的数量。
    /// </summary>
    int BufferBoostSubscriptionCount { get; }

    /// <summary>
    ///     获取此服务器中语音频道的最高比特率。
    /// </summary>
    /// <remarks>
    ///     此限制取决于服务器的助力状态。
    /// </remarks>
    int MaxBitrate { get; }

    /// <summary>
    ///     获取此服务器的文件上传限制，以字节为单位。
    /// </summary>
    /// <remarks>
    ///     此限制取决于服务器的助力状态。
    /// </remarks>
    ulong MaxUploadLimit { get; }

    /// <summary>
    ///     获取此服务器的服务器助力等级。
    /// </summary>
    BoostLevel BoostLevel { get; }

    /// <summary>
    ///     获取此服务器的所有自定义表情。
    /// </summary>
    IReadOnlyCollection<GuildEmote> Emotes { get; }

    /// <summary>
    ///     获取此服务器的所有角色。
    /// </summary>
    IReadOnlyCollection<IRole> Roles { get; }

    /// <summary>
    ///     获取此服务器的推荐信息。
    /// </summary>
    IRecommendInfo? RecommendInfo { get; }

    #endregion

    #region Guilds

    /// <summary>
    ///     退出此服务器。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步退出操作的任务。 </returns>
    Task LeaveAsync(RequestOptions? options = null);

    /// <summary>
    ///     所有此服务器的所有服务器助力包。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是所有为此服务器助力的用户及所应用的服务器助力包。 </returns>
    Task<ImmutableDictionary<IUser, IReadOnlyCollection<BoostSubscriptionMetadata>>> GetBoostSubscriptionsAsync(RequestOptions? options = null);

    /// <summary>
    ///     获取此服务器所有生效中的服务器助力包。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是所有为此服务器助力的用户及所应用的生效中的服务器助力包。 </returns>
    Task<ImmutableDictionary<IUser, IReadOnlyCollection<BoostSubscriptionMetadata>>> GetActiveBoostSubscriptionsAsync(RequestOptions? options = null);

    #endregion

    #region Guild Bans

    /// <summary>
    ///     获取此服务器的所有封禁信息。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器的所有封禁信息。 </returns>
    Task<IReadOnlyCollection<IBan>> GetBansAsync(RequestOptions? options = null);

    /// <summary>
    ///     获取指定用户在此服务器内当前的封禁信息。
    /// </summary>
    /// <param name="user"> 要获取封禁信息的用户。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含该用户在此服务器内的封禁信息；如果该用户当前未被此服务器封禁，则返回 <c>null</c>。 </returns>
    Task<IBan?> GetBanAsync(IUser user, RequestOptions? options = null);

    /// <summary>
    ///     获取指定用户在此服务器内的封禁信息。
    /// </summary>
    /// <param name="userId"> 要获取封禁信息的用户的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含该用户在此服务器内的封禁信息；如果该用户未被此服务器封禁，或封禁已过期或解除，则返回 <c>null</c>。 </returns>
    Task<IBan?> GetBanAsync(ulong userId, RequestOptions? options = null);

    /// <summary>
    ///     封禁服务器内的用户。
    /// </summary>
    /// <param name="user"> 要封禁的用户。 </param>
    /// <param name="pruneDays"> 要删除此服务器中来自此用户的最近几天的消息，范围为 <c>0</c> 至 <c>7</c>，<c>0</c> 表示不删除。 </param>
    /// <param name="reason"> 封禁原因。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <exception cref="ArgumentException"> <paramref name="pruneDays" /> 超出了 0 至 7 的范围。 </exception>
    /// <returns> 一个表示异步封禁操作的任务。 </returns>
    Task AddBanAsync(IUser user, int pruneDays = 0, string? reason = null, RequestOptions? options = null);

    /// <summary>
    ///     封禁服务器内的用户。
    /// </summary>
    /// <param name="userId"> 要封禁的用户的 ID。 </param>
    /// <param name="pruneDays"> 要删除此服务器中来自此用户的最近几天的消息，范围为 <c>0</c> 至 <c>7</c>，<c>0</c> 表示不删除。 </param>
    /// <param name="reason"> 封禁原因。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <exception cref="ArgumentException"> <paramref name="pruneDays" /> 超出了 0 至 7 的范围。 </exception>
    /// <returns> 一个表示异步封禁操作的任务。 </returns>
    Task AddBanAsync(ulong userId, int pruneDays = 0, string? reason = null, RequestOptions? options = null);

    /// <summary>
    ///     解除服务器对用户的封禁。
    /// </summary>
    /// <param name="user"> 要解除封禁的用户。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步封禁解除操作的任务。 </returns>
    Task RemoveBanAsync(IUser user, RequestOptions? options = null);

    /// <summary>
    ///     解除服务器对用户的封禁。
    /// </summary>
    /// <param name="userId"> 要解除封禁的用户的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步封禁解除操作的任务。 </returns>
    Task RemoveBanAsync(ulong userId, RequestOptions? options = null);

    #endregion

    #region Channels

    /// <summary>
    ///     获取此服务器的所有频道。
    /// </summary>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器的所有频道。 </returns>
    Task<IReadOnlyCollection<IGuildChannel>> GetChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取此服务器内的频道。
    /// </summary>
    /// <param name="id"> 要获取的频道的 ID。 </param>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含与指定的 <paramref name="id"/> 关联的频道；如果未找到，则返回 <c>null</c>。 </returns>
    Task<IGuildChannel?> GetChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取此服务器中所有具有文字聊天能力的频道。
    /// </summary>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器的所有具有文字聊天能力的频道。 </returns>
    /// <remarks>
    ///     语音频道也是一种文字频道，此方法本意用于获取所有具有文字聊天能力的频道，通过此方法获取到的文字频道列表中也包含了语音频道。
    ///     如需获取频道的实际类型，请参考 <see cref="M:Kook.ChannelExtensions.GetChannelType(Kook.IChannel)"/>。
    /// </remarks>
    Task<IReadOnlyCollection<ITextChannel>> GetTextChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取此服务器内指定具有文字聊天能力的频道。
    /// </summary>
    /// <param name="id"> 要获取的频道的 ID。 </param>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含与指定的 <paramref name="id"/> 关联的频道；如果未找到，则返回 <c>null</c>。 </returns>
    /// <remarks>
    ///     语音频道也是一种文字频道，此方法本意用于获取具有文字聊天能力的频道。如果通过此方法传入的 ID 对应的频道是语音频道，那么也会返回对应的语音频道实体。
    ///     如需获取频道的实际类型，请参考 <see cref="M:Kook.ChannelExtensions.GetChannelType(Kook.IChannel)"/>。
    /// </remarks>
    Task<ITextChannel?> GetTextChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取此服务器中所有具有语音聊天能力的频道。
    /// </summary>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器的所有具有语音聊天能力的频道。 </returns>
    Task<IReadOnlyCollection<IVoiceChannel>> GetVoiceChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取此服务器内指定具有语音聊天能力的频道。
    /// </summary>
    /// <param name="id"> 要获取的频道的 ID。 </param>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含与指定的 <paramref name="id"/> 关联的频道；如果未找到，则返回 <c>null</c>。 </returns>
    Task<IVoiceChannel?> GetVoiceChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取此服务器中的所有分组频道。
    /// </summary>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器的所有分组频道。 </returns>
    Task<IReadOnlyCollection<ICategoryChannel>> GetCategoryChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取此服务器的默认文字频道。
    /// </summary>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器的默认文字频道；如果未找到，则返回 <c>null</c>。 </returns>
    Task<ITextChannel?> GetDefaultChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取此服务器的欢迎通知频道。
    /// </summary>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器的欢迎通知频道；如果未找到，则返回 <c>null</c>。 </returns>
    Task<ITextChannel?> GetWelcomeChannelAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     在此服务器内创建一个新的文字频道。
    /// </summary>
    /// <param name="name"> 频道的名称。 </param>
    /// <param name="func"> 一个包含要应用到新创建频道的配置的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步创建操作的任务。任务的结果包含新创建的文字频道。 </returns>
    Task<ITextChannel> CreateTextChannelAsync(string name, Action<CreateTextChannelProperties>? func = null, RequestOptions? options = null);

    /// <summary>
    ///     在此服务器内创建一个新的语音频道。
    /// </summary>
    /// <param name="name"> 频道的名称。 </param>
    /// <param name="func"> 一个包含要应用到新创建频道的配置的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步创建操作的任务。任务的结果包含新创建的语音频道。 </returns>
    Task<IVoiceChannel> CreateVoiceChannelAsync(string name, Action<CreateVoiceChannelProperties>? func = null, RequestOptions? options = null);

    /// <summary>
    ///     在此服务器内创建一个新的分组频道。
    /// </summary>
    /// <param name="name"> 频道的名称。 </param>
    /// <param name="func"> 一个包含要应用到新创建频道的配置的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步创建操作的任务。任务的结果包含新创建的分组频道。 </returns>
    Task<ICategoryChannel> CreateCategoryChannelAsync(string name, Action<CreateCategoryChannelProperties>? func = null, RequestOptions? options = null);

    #endregion

    #region Invites

    /// <summary>
    ///     获取此服务器内的所有邀请信息。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器内的所有邀请信息。 </returns>
    Task<IReadOnlyCollection<IInvite>> GetInvitesAsync(RequestOptions? options = null);

    /// <summary>
    ///     创建一个到此服务器的新邀请。
    /// </summary>
    /// <param name="maxAge"> 邀请链接的有效时长，<see cref="F:Kook.InviteMaxAge.NeverExpires"/> 表示永不过期。 </param>
    /// <param name="maxUses"> 邀请链接的可用人次，<see cref="F:Kook.InviteMaxUses.Unlimited"/> 表示无限制。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步创建操作的任务。任务的结果包含新创建的邀请链接的元数据，其中包含有关邀请链接的信息。 </returns>
    Task<IInvite> CreateInviteAsync(InviteMaxAge maxAge = InviteMaxAge._604800, InviteMaxUses maxUses = InviteMaxUses.Unlimited, RequestOptions? options = null);

    /// <summary>
    ///     创建一个到此服务器的新邀请。
    /// </summary>
    /// <param name="maxAge"> 邀请链接的有效时长，<c>null</c> 表示永不过期。 </param>
    /// <param name="maxUses"> 邀请链接的可用人次，<c>null</c> 表示无限制。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步创建操作的任务。任务的结果包含新创建的邀请链接的元数据，其中包含有关邀请链接的信息。 </returns>
    Task<IInvite> CreateInviteAsync(int? maxAge = 604800, int? maxUses = null, RequestOptions? options = null);

    #endregion

    #region Roles

    /// <summary>
    ///     获取此服务器内的角色。
    /// </summary>
    /// <param name="id"> 要获取的角色的 ID。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含与指定的 <paramref name="id"/> 关联的角色；如果未找到，则返回 <c>null</c>。 </returns>
    IRole? GetRole(uint id);

    /// <summary>
    ///     在此服务器内创建一个新角色。
    /// </summary>
    /// <param name="name"> 角色的名称。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步创建操作的任务。任务的结果包含新创建的角色。 </returns>
    Task<IRole> CreateRoleAsync(string name, RequestOptions? options = null);

    #endregion

    #region Users

    /// <summary>
    ///     获取此服务器内的所有用户。
    /// </summary>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器内的所有用户。 </returns>
    Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取此服务器内的用户。
    /// </summary>
    /// <remarks>
    ///     此方法获取加入到此服务器内的用户。
    ///     <note>
    ///         此方法在 WebSocket 的实现中可能返回 <c>null</c>，因为在大型服务器中，用户列表的缓存可能不完整。
    ///     </note>
    /// </remarks>
    /// <param name="id"> 要获取的用户的 ID。 </param>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含与指定的 <paramref name="id"/> 关联的用户；如果未找到，则返回 <c>null</c>。 </returns>
    Task<IGuildUser?> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取此服务器内当前登录的用户。
    /// </summary>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器内当前登录的用户。 </returns>
    Task<IGuildUser?> GetCurrentUserAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取此服务器的所有者。
    /// </summary>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器的所有者。 </returns>
    Task<IGuildUser?> GetOwnerAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     下载此服务器内的所有用户。
    /// </summary>
    /// <remarks>
    ///     此方法会下载所有加入到此服务器内的用户，并缓存它们。
    /// </remarks>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步下载操作的任务。 </returns>
    Task DownloadUsersAsync(RequestOptions? options = null);

    /// <summary>
    ///     下载此服务器内的所有语音状态。
    /// </summary>
    /// <remarks>
    ///     此方法会下载此服务器内的所有语音状态，并缓存它们。
    /// </remarks>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步下载操作的任务。 </returns>
    Task DownloadVoiceStatesAsync(RequestOptions? options = null);

    /// <summary>
    ///     下载此服务器内的所有服务器助力信息。
    /// </summary>
    /// <remarks>
    ///     此方法会通过网关下载此服务器内的所有服务器助力信息，并缓存它们。
    ///     要下载所有服务器助力信息，当前用户必须具有 <see cref="F:Kook.GuildPermission.ManageGuild"/> 权限。
    /// </remarks>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步下载操作的任务。 </returns>
    Task DownloadBoostSubscriptionsAsync(RequestOptions? options = null);

    /// <summary>
    ///     搜索加入到此服务器内匹配指定搜索条件的所有用户。
    /// </summary>
    /// <remarks>
    ///     此方法使用指定的属性搜索服务器用户。要查看可用的属性，请参考 <see cref="T:Kook.SearchGuildMemberProperties"/>。
    /// </remarks>
    /// <param name="func"> 一个包含要搜索的用户属性及排序条件的委托。 </param>
    /// <param name="limit"> 搜索结果的最大数量。 </param>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含与提供的 <paramref name="func"/> 中指定的属性匹配的服务器用户集合。 </returns>
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> SearchUsersAsync(Action<SearchGuildMemberProperties> func,
        int limit = KookConfig.MaxUsersPerBatch, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    #endregion

    #region Emotes

    /// <summary>
    ///     获取此服务器的所有自定义表情。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器的所有自定义表情。 </returns>
    Task<IReadOnlyCollection<GuildEmote>> GetEmotesAsync(RequestOptions? options = null);

    /// <summary>
    ///     获取此服务器的指定自定义表情。
    /// </summary>
    /// <param name="id"> 要获取的自定义表情的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含与指定的 <paramref name="id"/> 关联的自定义表情；如果未找到，则返回 <c>null</c>。 </returns>
    Task<GuildEmote?> GetEmoteAsync(string id, RequestOptions? options = null);

    /// <summary>
    ///     在此服务器内创建一个新的自定义表情。
    /// </summary>
    /// <param name="name"> 新自定义表情的名称。 </param>
    /// <param name="image"> 新自定义表情的图像信息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步创建操作的任务。任务的结果包含新创建的自定义表情。 </returns>
    Task<GuildEmote> CreateEmoteAsync(string name, Image image, RequestOptions? options = null);

    /// <summary>
    ///     修改此服务器内的现有自定义表情。
    /// </summary>
    /// <param name="emote"> 要修改的自定义表情。 </param>
    /// <param name="name"> 新的自定义表情名称。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。任务的结果包含修改后的自定义表情。 </returns>
    Task ModifyEmoteNameAsync(GuildEmote emote, string name, RequestOptions? options = null);

    /// <summary>
    ///     删除此服务器内的现有自定义表情。
    /// </summary>
    /// <param name="emote"> 要删除的自定义表情。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步删除操作的任务。 </returns>
    Task DeleteEmoteAsync(GuildEmote emote, RequestOptions? options = null);

    #endregion

    #region Voice

    /// <summary>
    ///     移动用户到语音频道。
    /// </summary>
    /// <param name="users"> 要移动的用户。 </param>
    /// <param name="targetChannel"> 要移动用户到的语音频道。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步移动操作的任务。 </returns>
    Task MoveUsersAsync(IEnumerable<IGuildUser> users, IVoiceChannel targetChannel, RequestOptions? options = null);

    #endregion

    #region Badges

    /// <summary>
    ///     获取与此服务器关联的徽章。
    /// </summary>
    /// <param name="style"> 要获取的徽章的样式。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含与此服务器关联的徽章的流。 </returns>
    Task<Stream> GetBadgeAsync(BadgeStyle style = BadgeStyle.GuildName, RequestOptions? options = null);

    #endregion
}
