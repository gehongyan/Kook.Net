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
    /// <returns> 一个表示异步操作的任务，任务的结果是服务器的所有行为限制信息。 </returns>
    /// <exception cref="NotSupportedException">
    ///     当 <paramref name="guild"/> 不是 <see cref="Kook.Rest.RestGuild"/> 或
    ///     <see cref="Kook.WebSocket.SocketGuild"/> 时引发。
    /// </exception>
    public static async Task<IReadOnlyCollection<RestBehaviorRestriction>> GetBehaviorRestrictionsAsync(
        this IGuild guild, RequestOptions? options = null) => guild switch
    {
        RestGuild restGuild => await RestGuildExperimentalExtensions.GetBehaviorRestrictionsAsync(restGuild, options),
        SocketGuild socketGuild => await SocketGuildExperimentalExtensions.GetBehaviorRestrictionsAsync(socketGuild,
            options),
        _ => throw new NotSupportedException(
            "Experimental guild extensions are only supported for RestGuild and SocketGuild."),
    };

    /// <summary>
    ///     创建服务器行为限制。
    /// </summary>
    /// <param name="guild"> 要创建行为限制的服务器。 </param>
    /// <param name="name"> 行为限制名称。 </param>
    /// <param name="conditions"> 行为限制条件集合。 </param>
    /// <param name="duration"> 行为限制持续时间。 </param>
    /// <param name="restrictionType"> 行为限制类型。 </param>
    /// <param name="isEnabled"> 是否启用行为限制。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步操作的任务，任务的结果是创建的行为限制。 </returns>
    public static async Task<RestBehaviorRestriction> CreateBehaviorRestrictionAsync(
        this IGuild guild, string name, IReadOnlyCollection<IBehaviorRestrictionCondition> conditions,
        TimeSpan duration, BehaviorRestrictionType restrictionType, bool isEnabled,
        RequestOptions? options = null) => guild switch
    {
        RestGuild restGuild => await RestGuildExperimentalExtensions.CreateBehaviorRestrictionAsync(
            restGuild, name, conditions, duration, restrictionType, isEnabled, options),
        SocketGuild socketGuild => await SocketGuildExperimentalExtensions.CreateBehaviorRestrictionAsync(
            socketGuild, name, conditions, duration, restrictionType, isEnabled, options),
        _ => throw new NotSupportedException(
            "Experimental guild extensions are only supported for RestGuild and SocketGuild."),
    };
}
