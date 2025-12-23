namespace Kook;

/// <summary>
///     表示一个通用的服务器用户。
/// </summary>
public interface IGuildUser : IUser, IVoiceState
{
    #region General

    /// <summary>
    ///     获取此用户在该服务器内的昵称。
    /// </summary>
    /// <remarks>
    ///     如果此用户在该服务器内没有设置昵称，则此属性返回 <see langword="null"/>。
    /// </remarks>
    string? Nickname { get; }

    /// <summary>
    ///     获取此用户的显示名称。
    /// </summary>
    /// <remarks>
    ///     如果此用户在该服务器内设置了昵称，则此属性返回昵称；否则返回用户名。
    /// </remarks>
    string DisplayName { get; }

    /// <summary>
    ///     获取此用户在该服务器内拥有的所有角色的 ID。
    /// </summary>
    /// <remarks>
    ///     此属性返回此用户所拥有的所有角色的 ID。对于 WebSocket 服务器用户实体，<c>Roles</c> 属性可以用来获取所有角色对象；对于 REST
    ///     服务器用户实体，受限于 KOOK API，在服务器用户实体上仅能直接获取其所拥有的所有角色的 ID。
    /// </remarks>
    IReadOnlyCollection<uint> RoleIds { get; }

    /// <summary>
    ///     获取此服务器用户所属的服务器。
    /// </summary>
    IGuild Guild { get; }

    /// <summary>
    ///     获取此用户所属服务器的 ID。
    /// </summary>
    ulong GuildId { get; }

    /// <summary>
    ///     获取此用户的手机号码是否已验证。
    /// </summary>
    bool? IsMobileVerified { get; }

    /// <summary>
    ///     获取此用户加入服务器的时间。
    /// </summary>
    DateTimeOffset? JoinedAt { get; }

    /// <summary>
    ///     获取此用户在该服务器内的最近活跃时间。
    /// </summary>
    DateTimeOffset? ActiveAt { get; }

    /// <summary>
    ///     获取此用户的显示名称的颜色。
    /// </summary>
    /// <remarks>
    ///     如果此用户所拥有的最高角色的颜色类型为渐变色，则此属性返回的颜色是渐变色权益失效后的回退颜色。 <br />
    ///     <note type="warning">
    ///         一个角色的颜色可能是纯色或渐变色，参见 <see cref="Kook.IRole.ColorType"/>。但由于服务器用户列表 API
    ///         及服务器用户详情 API 所返回的用户信息均不包含角色的颜色类型和渐变色信息，因此，如果用户的最高角色的颜色是渐变色，
    ///         则此属性的值可能是不正确的。如需获取该用户的准确的显示名称及颜色，请获取此用户的最高角色实体对象，访问其颜色类型及渐变色属性。
    ///     </note>
    ///     <code language="cs">
    ///         if (guildUser.RoleIds.Select(x =&gt; guildUser.Guild.GetRole(x)).OfType&lt;IRole&gt;().MinBy(x =&gt; x.Position) is { } topRole)
    ///         {
    ///             ColorType colorType = topRole.ColorType;
    ///             GradientColor? gradientColor = topRole.GradientColor;
    ///         }
    ///     </code>
    /// </remarks>
    Color? Color { get; }

    /// <summary>
    ///     获取此用户是否为当前服务器的所有者。
    /// </summary>
    bool? IsOwner { get; }

    #endregion

    #region Permissions

    /// <summary>
    ///     获取此用户在该服务器内的权限。
    /// </summary>
    GuildPermissions GuildPermissions { get; }

    /// <summary>
    ///     获取此用户在指定频道内所拥有的权限。
    /// </summary>
    /// <param name="channel"> 要获取权限的频道。 </param>
    /// <returns> 一个表示此用户在指定频道内所拥有的频道权限的权限集。 </returns>
    ChannelPermissions GetPermissions(IGuildChannel channel);

    #endregion

    #region Guild

    /// <summary>
    ///     将此用户从此服务器中踢出。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步踢出操作的任务。 </returns>
    Task KickAsync(RequestOptions? options = null);

    /// <summary>
    ///     修改此用户在该服务器内的昵称。
    /// </summary>
    /// <remarks>
    ///     此方法使用指定的属性修改当前用户在该服务器内的昵称。 <br />
    ///     如要清除此用户在该服务器内的昵称，请将 <paramref name="name"/> 设置为 <see langword="null"/>。 <br />
    ///     <note type="warning">
    ///         如果将昵称设置为与用户名相同，KOOK 也会将该用户在此服务器内的昵称清除，显示名称将跟随用户名，而不是固定为指定的昵称。
    ///     </note>
    /// </remarks>
    /// <param name="name"> 要设置到此用户在该服务器内的新昵称。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。 </returns>
    Task ModifyNicknameAsync(string? name, RequestOptions? options = null);

    /// <summary>
    ///     获取此用户在该服务器内的所有服务器助力包订阅信息。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此用户在该服务器内的所有服务器助力包订阅信息。 </returns>
    Task<IReadOnlyCollection<BoostSubscriptionMetadata>> GetBoostSubscriptionsAsync(RequestOptions? options = null);

    #endregion

    #region Roles

    /// <summary>
    ///     在该服务器内授予此用户指定的角色。
    /// </summary>
    /// <param name="roleId"> 要在该服务器内为此用户授予的角色的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步授予操作的任务。 </returns>
    Task AddRoleAsync(uint roleId, RequestOptions? options = null);

    /// <summary>
    ///     在该服务器内授予此用户指定的角色。
    /// </summary>
    /// <param name="role"> 要在该服务器内为此用户授予的角色。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步授予操作的任务。 </returns>
    Task AddRoleAsync(IRole role, RequestOptions? options = null);

    /// <summary>
    ///     在该服务器内授予此用户指定的一些角色。
    /// </summary>
    /// <param name="roleIds"> 要在该服务器内为此用户授予的所有角色的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步授予操作的任务。 </returns>
    Task AddRolesAsync(IEnumerable<uint> roleIds, RequestOptions? options = null);

    /// <summary>
    ///     在该服务器内授予此用户指定的一些角色。
    /// </summary>
    /// <param name="roles"> 要在该服务器内为此用户授予的所有角色。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步授予操作的任务。 </returns>
    Task AddRolesAsync(IEnumerable<IRole> roles, RequestOptions? options = null);

    /// <summary>
    ///     在该服务器内撤销此用户指定的角色。
    /// </summary>
    /// <param name="roleId"> 要在该服务器内为此用户撤销的角色的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步撤销操作的任务。 </returns>
    Task RemoveRoleAsync(uint roleId, RequestOptions? options = null);

    /// <summary>
    ///     在该服务器内撤销此用户指定的角色。
    /// </summary>
    /// <param name="role"> 要在该服务器内为此用户撤销的角色。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步撤销操作的任务。 </returns>
    Task RemoveRoleAsync(IRole role, RequestOptions? options = null);

    /// <summary>
    ///     在该服务器内撤销此用户指定的一些角色。
    /// </summary>
    /// <param name="roleIds"> 要在该服务器内为此用户撤销的所有角色的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步撤销操作的任务。 </returns>
    Task RemoveRolesAsync(IEnumerable<uint> roleIds, RequestOptions? options = null);

    /// <summary>
    ///     在该服务器内撤销此用户指定的一些角色。
    /// </summary>
    /// <param name="roles"> 要在该服务器内为此用户撤销的所有角色。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步撤销操作的任务。 </returns>
    Task RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions? options = null);

    #endregion

    #region Voice

    /// <summary>
    ///     在该服务器内关闭此用户的语音输入。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步关闭操作的任务。 </returns>
    /// <remarks>
    ///     此操作会使此用户无法在该服务器内的语音频道中发言。
    /// </remarks>
    Task MuteAsync(RequestOptions? options = null);

    /// <summary>
    ///     在该服务器内限制此用户的语音接收。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步静音操作的任务。 </returns>
    /// <remarks>
    ///     此操作会使此用户无法在该服务器内的语音频道中接收来自其他用户的语音。
    /// </remarks>
    Task DeafenAsync(RequestOptions? options = null);

    /// <summary>
    ///     在该服务器内恢复此用户的语音输入。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步恢复操作的任务。 </returns>
    /// <remarks>
    ///     此操作会撤销由于服务器闭麦而导致的在语音频道中无法发言状态。
    /// </remarks>
    Task UnmuteAsync(RequestOptions? options = null);

    /// <summary>
    ///     在该服务器内恢复此用户的语音接收。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步恢复操作的任务。 </returns>
    /// <remarks>
    ///     此操作会撤销由于服务器静音而导致的无法在语音频道中接收来自其他用户的语音的状态。
    /// </remarks>
    Task UndeafenAsync(RequestOptions? options = null);

    /// <summary>
    ///     获取此用户当前所连接到的所有语音频道。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此用户当前所连接到的所有语音频道。 </returns>
    Task<IReadOnlyCollection<IVoiceChannel>> GetConnectedVoiceChannelsAsync(RequestOptions? options = null);

    /// <summary>
    ///     将此用户从语音频道中断开连接。
    /// </summary>
    /// <param name="channel"> 要断开连接的语音频道。如果为 <c>null</c>，则会将用户从当前连接的所有语音频道中断开连接。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步断开连接操作的任务。 </returns>
    Task DisconnectAsync(IVoiceChannel? channel = null, RequestOptions? options = null);

    #endregion
}
