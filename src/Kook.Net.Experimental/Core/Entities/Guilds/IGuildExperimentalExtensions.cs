using Kook.Rest;
using Kook.WebSocket;

namespace Kook;

/// <summary>
///     提供对 <see cref="Kook.IGuild"/> 的实验性方法。
/// </summary>
public static class GuildExperimentalExtensions
{
    /// <summary>
    ///     获取服务器的行为限制信息。
    /// </summary>
    /// <param name="guild"> 要获取行为限制信息的服务器。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 服务器行为限制信息。 </returns>
    /// <exception cref="NotSupportedException">
    ///     当 <paramref name="guild"/> 不是 <see cref="Kook.Rest.RestGuild"/> 或
    ///     <see cref="Kook.WebSocket.SocketGuild"/> 时引发。
    /// </exception>
    public static async Task<IReadOnlyCollection<RestGuildBehaviorRestriction>> GetBehaviorRestrictionsAsync(
        this IGuild guild, RequestOptions? options = null) => guild switch
    {
        RestGuild restGuild => await RestGuildExperimentalExtensions.GetBehaviorRestrictionsAsync(restGuild, options),
        SocketGuild socketGuild => await SocketGuildExperimentalExtensions.GetBehaviorRestrictionsAsync(socketGuild, options),
        _ => throw new NotSupportedException("Experimental guild extensions are only supported for RestGuild and SocketGuild."),
    };
}
