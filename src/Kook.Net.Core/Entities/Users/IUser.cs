namespace Kook;

/// <summary>
///     表示一个通用的用户。
/// </summary>
public interface IUser : IEntity<ulong>, IMentionable, IPresence
{
    /// <summary>
    ///     获取此用户的用户名。
    /// </summary>
    string Username { get; }

    /// <summary>
    ///     获取此用户的识别号。
    /// </summary>
    string IdentifyNumber { get; }

    /// <summary>
    ///     获取此用户识别号的数值形式。
    /// </summary>
    ushort IdentifyNumberValue { get; }

    /// <summary>
    ///     获取此用户是否为 Bot。
    /// </summary>
    /// <remarks>
    ///     如果未知此用户是否为 Bot，则此属性返回 <see langword="null"/>。
    /// </remarks>
    bool? IsBot { get; }

    /// <summary>
    ///     获取此用户是否被封禁。
    /// </summary>
    /// <remarks>
    ///     如果未知此用户是否被封禁，则此属性返回 <see langword="null"/>。
    /// </remarks>
    bool? IsBanned { get; }

    /// <summary>
    ///     获取此用户是否订阅了 BUFF 会员。
    /// </summary>
    /// <remarks>
    ///     如果未知此用户是否订阅了 BUFF 会员，则此属性返回 <see langword="null"/>。
    /// </remarks>
    bool? HasBuff { get; }

    /// <summary>
    ///     获取此用户是否订阅了年度 BUFF 会员。
    /// </summary>
    /// <remarks>
    ///     如果未知此用户是否订阅了年度 BUFF 会员，则此属性返回 <see langword="null"/>。
    /// </remarks>
    bool? HasAnnualBuff { get; }

    /// <summary>
    ///     获取此用户的头像图像的 URL。
    /// </summary>
    /// <remarks>
    ///     如果此用户为 BUFF 会员，且以 BUFF 会员权益设置了头像，则此属性返回的 URL 表示的是 BUFF 专属头像权益时效后的回退头像。
    /// </remarks>
    string Avatar { get; }

    /// <summary>
    ///     获取此用户以 BUFF 会员权益设置的头像图像的 URL。
    /// </summary>
    /// <remarks>
    ///     如果此用户不是 BUFF 会员，或未以 BUFF 会员权限设置头像，则此属性返回 <see langword="null"/>。
    /// </remarks>
    string? BuffAvatar { get; }

    /// <summary>
    ///     获取此用户的横幅图像的 URL。
    /// </summary>
    /// <remarks>
    ///     如果此用户不是 BUFF 会员，或未以 BUFF 会员权限设置横幅，则此属性返回 <see langword="null"/>。
    /// </remarks>
    string? Banner { get; }

    /// <summary>
    ///     获取此用户是否启用了降噪功能。
    /// </summary>
    /// <remarks>
    ///     如果未知此用户是否启用了降噪功能，则此属性返回 <see langword="null"/>。
    /// </remarks>
    bool? IsDenoiseEnabled { get; }

    /// <summary>
    ///     获取此用户的标签信息。
    /// </summary>
    /// <remarks>
    ///     用户的标签会显示在服务器用户列表、私信列表、私信消息页、好友列表、个人信息面板、聊天消息的用户名的右侧。 <br />
    ///     如果此用户没有标签，或未知此用户的标签信息，则此属性返回 <see langword="null"/>。
    /// </remarks>
    UserTag? UserTag { get; }

    /// <summary>
    ///     获取此用户设置展示的所有铭牌。
    /// </summary>
    /// <remarks>
    ///     用户设置的首个铭牌会展示在该用户聊天消息的用户名的右侧，用户设置的所有铭牌会展示在个人信息面板内的用户名下方。
    /// </remarks>
    IReadOnlyCollection<Nameplate> Nameplates { get; }

    /// <summary>
    ///     获取此用户是否为系统用户。
    /// </summary>
    bool IsSystemUser { get; }

    /// <summary>
    ///     创建一个用于与此用户收发私信的频道。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步创建操作的任务。任务结果包含与此用户相关的私信频道。 </returns>
    Task<IDMChannel> CreateDMChannelAsync(RequestOptions? options = null);

    /// <summary>
    ///     获取与此用户的亲密度信息。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务结果包含与此用户的亲密度信息。 </returns>
    Task<IIntimacy> GetIntimacyAsync(RequestOptions? options = null);

    /// <summary>
    ///     修改与此用户的亲密度信息。
    /// </summary>
    /// <remarks>
    ///     此方法使用指定的属性修改与此用户的亲密度信息。要查看可用的属性，请参考 <see cref="Kook.IntimacyProperties"/>。
    /// </remarks>
    /// <param name="func"> 一个用于修改亲密度信息的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。 </returns>
    Task UpdateIntimacyAsync(Action<IntimacyProperties> func, RequestOptions? options = null);

    /// <summary>
    ///     屏蔽此用户。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步屏蔽操作的任务。 </returns>
    Task BlockAsync(RequestOptions? options = null);

    /// <summary>
    ///     取消屏蔽此用户。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步取消操作的任务。 </returns>
    Task UnblockAsync(RequestOptions? options = null);

    /// <summary>
    ///     向此用户发送好友请求。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步发送操作的任务。 </returns>
    Task RequestFriendAsync(RequestOptions? options = null);

    /// <summary>
    ///     移除与此用户的好友关系。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步移除操作的任务。 </returns>
    Task RemoveFriendAsync(RequestOptions? options = null);

    /// <summary>
    ///     请求与此用户建立亲密关系。
    /// </summary>
    /// <param name="relationType"> 要请求的亲密关系类型。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步请求操作的任务。 </returns>
    Task RequestIntimacyRelationAsync(IntimacyRelationType relationType, RequestOptions? options = null);

    /// <summary>
    ///     解除与此用户的亲密关系。
    /// </summary>
    /// <param name="removeFriend"> 是否同时移除与此用户的好友关系。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步解除操作的任务。 </returns>
    Task UnravelIntimacyRelationAsync(bool removeFriend = false, RequestOptions? options = null);
}
