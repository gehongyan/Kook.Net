namespace Kook;

/// <summary>
///     提供一组用于生成 KOOK 客户端跳转链接的辅助方法。
/// </summary>
public static class DirectLinks
{
    private const string BaseUrl = "https://www.kookapp.cn/direct";

    /// <summary>
    ///     返回工单页面的跳转链接。
    /// </summary>
    /// <returns> 工单页面的跳转链接。 </returns>
    public static string Feedback() => $"{BaseUrl}/feedback";

    /// <summary>
    ///     返回消息锚点的跳转链接。
    /// </summary>
    /// <param name="guildId"> 服务器 ID。 </param>
    /// <param name="channelId"> 频道 ID。 </param>
    /// <param name="messageId"> 消息 ID。 </param>
    /// <returns> 消息锚点的跳转链接。 </returns>
    public static string MessageAnchor(ulong guildId, ulong channelId, Guid messageId) =>
        $"{BaseUrl}/anchor/{guildId}/{channelId}/{messageId:D}";

    /// <summary>
    ///     返回消息锚点的跳转链接。
    /// </summary>
    /// <param name="guildId"> 服务器 ID。 </param>
    /// <param name="channelId"> 频道 ID。 </param>
    /// <param name="threadId"> 帖子 ID。 </param>
    /// <returns> 消息锚点的跳转链接。 </returns>
    public static string MessageAnchor(ulong guildId, ulong channelId, ulong threadId) =>
        $"{BaseUrl}/anchor/{guildId}/{channelId}/{threadId}";

    /// <summary>
    ///     返回消息锚点的跳转链接。
    /// </summary>
    /// <param name="guildId"> 服务器 ID。 </param>
    /// <param name="channelId"> 频道 ID。 </param>
    /// <param name="threadId"> 帖子 ID。 </param>
    /// <param name="messageId"> 评论或回复 ID。 </param>
    /// <returns> 消息锚点的跳转链接。 </returns>
    public static string MessageAnchor(ulong guildId, ulong channelId, ulong threadId, ulong messageId) =>
        $"{BaseUrl}/anchor/{guildId}/{channelId}/{threadId}/{messageId}";

    /// <summary>
    ///     返回实名认证页面的跳转链接。
    /// </summary>
    /// <returns> 实名认证页面的跳转链接。 </returns>
    public static string IdVerification() => $"{BaseUrl}/idverification";

    /// <summary>
    ///     返回服务器频道的跳转链接。
    /// </summary>
    /// <param name="guildId"> 服务器 ID。 </param>
    /// <param name="channelId"> 频道 ID。 </param>
    /// <returns> 服务器频道的跳转链接。 </returns>
    public static string Channel(ulong guildId, ulong channelId) =>
        $"{BaseUrl}/channel?g={guildId}&c={channelId}";

    /// <summary>
    ///     返回账号设置页面的跳转链接。
    /// </summary>
    /// <returns> 账号设置页面的跳转链接。 </returns>
    public static string MyAccount() => $"{BaseUrl}/myaccount";

    /// <summary>
    ///     返回游戏或活动页的跳转链接。
    /// </summary>
    /// <returns> 游戏或活动页的跳转链接。 </returns>
    public static string Activity() => $"{BaseUrl}/activity";

    /// <summary>
    ///     返回我的背包页面的跳转链接。
    /// </summary>
    /// <returns> 我的背包页面的跳转链接。 </returns>
    public static string MyInventory() => $"{BaseUrl}/myinventory";

    /// <summary>
    ///     返回隐私设置页面的跳转链接。
    /// </summary>
    /// <returns> 隐私设置页面的跳转链接。 </returns>
    public static string Privacy() => $"{BaseUrl}/privacy";

    /// <summary>
    ///     返回授权管理页面的跳转链接。
    /// </summary>
    /// <returns> 授权管理页面的跳转链接。 </returns>
    public static string AuthorizedApps() => $"{BaseUrl}/authorizedapps";

    /// <summary>
    ///     返回邀请页面的跳转链接。
    /// </summary>
    /// <returns> 邀请页面的跳转链接。 </returns>
    public static string Invitations() => $"{BaseUrl}/invitations";

    /// <summary>
    ///     返回激活 BUFF 页面的跳转链接。
    /// </summary>
    /// <returns> 激活 BUFF 页面的跳转链接。 </returns>
    public static string ActivateKookBuff() => $"{BaseUrl}/activatekookbuff";

    /// <summary>
    ///     返回服务器助力页面的跳转链接。
    /// </summary>
    /// <returns> 服务器助力页面的跳转链接。 </returns>
    public static string ServerBoost() => $"{BaseUrl}/serverboost";

    /// <summary>
    ///     返回道具商城的跳转链接。
    /// </summary>
    /// <returns> 道具商城的跳转链接。 </returns>
    public static string ItemShop() => $"{BaseUrl}/itemshop";

    /// <summary>
    ///     返回道具商城指定商品的跳转链接。
    /// </summary>
    /// <param name="goodsId"> 商品 ID。 </param>
    /// <returns> 道具商城指定商品的跳转链接。 </returns>
    public static string ItemShop(int goodsId) => $"{BaseUrl}/itemshop?goodsId={goodsId}";

    /// <summary>
    ///     返回我的账单页面的跳转链接。
    /// </summary>
    /// <returns> 我的账单页面的跳转链接。 </returns>
    public static string Billing() => $"{BaseUrl}/billing";

    /// <summary>
    ///     返回兑换码页面的跳转链接。
    /// </summary>
    /// <returns> 兑换码页面的跳转链接。 </returns>
    public static string RedeemCode() => $"{BaseUrl}/redeemcode";

    /// <summary>
    ///     返回语音设置页面的跳转链接。
    /// </summary>
    /// <returns> 语音设置页面的跳转链接。 </returns>
    public static string VoiceSettings() => $"{BaseUrl}/voicesettings";

    /// <summary>
    ///     返回按键设置页面的跳转链接。
    /// </summary>
    /// <returns> 按键设置页面的跳转链接。 </returns>
    public static string Keybinds() => $"{BaseUrl}/keybinds";

    /// <summary>
    ///     返回表情管理页面的跳转链接。
    /// </summary>
    /// <returns> 表情管理页面的跳转链接。 </returns>
    public static string Stickers() => $"{BaseUrl}/stickers";

    /// <summary>
    ///     返回游戏内覆盖页面的跳转链接。
    /// </summary>
    /// <returns> 游戏内覆盖页面的跳转链接。 </returns>
    public static string InGameOverlay() => $"{BaseUrl}/ingameoverlay";

    /// <summary>
    ///     返回通知页面的跳转链接。
    /// </summary>
    /// <returns> 通知页面的跳转链接。 </returns>
    public static string Notifications() => $"{BaseUrl}/notifications";

    /// <summary>
    ///     返回语言设置页面的跳转链接。
    /// </summary>
    /// <returns> 语言设置页面的跳转链接。 </returns>
    public static string Language() => $"{BaseUrl}/language";

    /// <summary>
    ///     返回外观设置页面的跳转链接。
    /// </summary>
    /// <returns> 外观设置页面的跳转链接。 </returns>
    public static string Appearance() => $"{BaseUrl}/appearance";

    /// <summary>
    ///     返回工具页面的跳转链接。
    /// </summary>
    /// <returns> 工具页面的跳转链接。 </returns>
    public static string Tools() => $"{BaseUrl}/tools";

    /// <summary>
    ///     返回主播模式页面的跳转链接。
    /// </summary>
    /// <returns> 主播模式页面的跳转链接。 </returns>
    public static string StreamerMode() => $"{BaseUrl}/streamermode";

    /// <summary>
    ///     返回高级设置页面的跳转链接。
    /// </summary>
    /// <returns> 高级设置页面的跳转链接。 </returns>
    public static string Advanced() => $"{BaseUrl}/advanced";

    /// <summary>
    ///     返回用户动态页面的跳转链接。
    /// </summary>
    /// <returns> 用户动态页面的跳转链接。 </returns>
    public static string ActivityStatus() => $"{BaseUrl}/activitystatus";

    /// <summary>
    ///     返回 Windows 设置页面的跳转链接。
    /// </summary>
    /// <returns> Windows 设置页面的跳转链接。 </returns>
    public static string WindowsSettings() => $"{BaseUrl}/windowssettings";

    /// <summary>
    ///     返回更新记录页面的跳转链接。
    /// </summary>
    /// <returns> 更新记录页面的跳转链接。 </returns>
    public static string ReleaseNotes() => $"{BaseUrl}/releasenotes";

    /// <summary>
    ///     返回代币充值弹框的跳转链接。
    /// </summary>
    /// <returns> 代币充值弹框的跳转链接。 </returns>
    public static string Deposit() => $"{BaseUrl}/deposit";

    /// <summary>
    ///     返回安装加速器弹框的跳转链接。
    /// </summary>
    /// <returns> 安装加速器弹框的跳转链接。 </returns>
    /// <remarks>
    ///     <note type="warning">
    ///         此链接会直接安装或启动加速器。
    ///     </note>
    /// </remarks>
    public static string KookBooster() => $"{BaseUrl}/kookbooster";

    /// <summary>
    ///     返回福利中心页面的跳转链接。
    /// </summary>
    /// <returns> 福利中心页面的跳转链接。 </returns>
    public static string EventCenter() => $"{BaseUrl}/eventcenter";

    /// <summary>
    ///     返回打开外部链接的跳转链接。
    /// </summary>
    /// <param name="externalUrl"> 要打开的外部链接。 </param>
    /// <returns> 打开外部链接的跳转链接。 </returns>
    public static string Link(string externalUrl) => $"{BaseUrl}/link?external={externalUrl}";

    /// <inheritdoc cref="DirectLinks.Link(System.String)" />
    public static string Link(Uri externalUrl) => Link(externalUrl.ToString());
}

