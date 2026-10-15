using Kook.Audio.WebRtc;

namespace Kook.Rest;

/// <summary>
///     提供用于 <see cref="Kook.Rest.KookRestClient"/> 的实验性功能的扩展方法。
/// </summary>
public static class KookRestClientExperimentalExtensions
{
    /// <summary>
    ///     获取当前用户具有 <see cref="Kook.GuildPermission.Administrator"/> 权限的服务器的集合。
    /// </summary>
    /// <param name="client"> KOOK REST 客户端实例。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步操作的任务，任务的结果包含当前用户具有 <see cref="Kook.GuildPermission.Administrator"/> 权限的使用服务器。 </returns>
    public static Task<IReadOnlyCollection<RestGuild>> GetAdminGuildsAsync(this KookRestClient client, RequestOptions? options = null) =>
        ExperimentalClientHelper.GetAdminGuildsAsync(client, options);

    /// <summary>
    ///     验证卡片。
    /// </summary>
    /// <param name="client"> KOOK REST 客户端实例。 </param>
    /// <param name="card"> 要验证的卡片。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步验证操作的任务。 </returns>
    public static Task ValidateCardAsync(this KookRestClient client, ICard card, RequestOptions? options = null) =>
        ValidateCardsAsync(client, [card], options);

    /// <summary>
    ///     验证卡片。
    /// </summary>
    /// <param name="client"> KOOK REST 客户端实例。 </param>
    /// <param name="cards"> 要验证的卡片。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步验证操作的任务。 </returns>
    public static Task ValidateCardsAsync(this KookRestClient client, IEnumerable<ICard> cards, RequestOptions? options = null) =>
        ExperimentalClientHelper.ValidateCardsAsync(client, cards, options);

    /// <summary>
    ///     验证卡片。
    /// </summary>
    /// <param name="client"> KOOK REST 客户端实例。 </param>
    /// <param name="cardsJson"> 要验证的卡片的 JSON 字符串。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步验证操作的任务。 </returns>
    public static Task ValidateCardsAsync(this KookRestClient client, string cardsJson, RequestOptions? options = null) =>
        ExperimentalClientHelper.ValidateCardsAsync(client, cardsJson, options);

    /// <summary>
    ///     查询与指定关键字相关的帖子话题标签。
    /// </summary>
    /// <param name="client"> KOOK REST 客户端实例。</param>
    /// <param name="keyword"> 要查询的关键字。</param>
    /// <param name="options"> 发送请求时要使用的选项。</param>
    /// <returns> 一个表示异步操作的任务，任务的结果包含与指定关键字相关的帖子话题标签集合。</returns>
    public static Task<IReadOnlyCollection<ThreadTag>> QueryThreadTagsAsync(this KookRestClient client,
        string keyword, RequestOptions? options = null) =>
        ExperimentalClientHelper.QueryThreadTagsAsync(client, keyword, options);

    /// <summary>
    ///     获取指定语音频道的语音信令网关令牌。
    /// </summary>
    /// <remarks>
    ///     此接口并非官方文档化的接口，行为可能随时变化。返回的令牌包含临时鉴权信息，请勿将其写入日志。
    ///     Bot 需要被授权才能获取令牌：实测中，没有任何服务器角色的 Bot 会被拒绝（HTTP 403）。
    ///     通常不需要直接调用此方法，使用 <see cref="Kook.Audio.WebRtc.SocketVoiceChannelExperimentalExtensions.ConnectWebRtcAsync(Kook.WebSocket.SocketVoiceChannel,System.Action{Kook.Audio.WebRtc.IWebRtcAudioClient})"/> 即可。
    /// </remarks>
    /// <param name="client"> KOOK REST 客户端实例。 </param>
    /// <param name="channelId"> 语音频道的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步操作的任务，任务的结果包含语音信令网关令牌。 </returns>
    public static Task<VoiceGatewayToken> GetVoiceGatewayTokenAsync(this KookRestClient client,
        ulong channelId, RequestOptions? options = null) =>
        ExperimentalClientHelper.GetVoiceGatewayTokenAsync(client, channelId, options);

    /// <summary>
    ///     获取指定语音频道的语音信令网关令牌。
    /// </summary>
    /// <remarks>
    ///     此接口并非官方文档化的接口，行为可能随时变化。返回的令牌包含临时鉴权信息，请勿将其写入日志。
    ///     Bot 需要被授权才能获取令牌：实测中，没有任何服务器角色的 Bot 会被拒绝（HTTP 403）。
    ///     通常不需要直接调用此方法，使用 <see cref="Kook.Audio.WebRtc.SocketVoiceChannelExperimentalExtensions.ConnectWebRtcAsync(Kook.WebSocket.SocketVoiceChannel,System.Action{Kook.Audio.WebRtc.IWebRtcAudioClient})"/> 即可。
    /// </remarks>
    /// <param name="client"> KOOK REST 客户端实例。 </param>
    /// <param name="channel"> 语音频道。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步操作的任务，任务的结果包含语音信令网关令牌。 </returns>
    public static Task<VoiceGatewayToken> GetVoiceGatewayTokenAsync(this KookRestClient client,
        IVoiceChannel channel, RequestOptions? options = null) =>
        ExperimentalClientHelper.GetVoiceGatewayTokenAsync(client, channel.Id, options);
}
