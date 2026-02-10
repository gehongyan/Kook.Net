using Kook.Rest;

namespace Kook;

/// <summary>
///     提供对 <see cref="Kook.IGuildBehaviorRestriction"/> 的实验性方法。
/// </summary>
public static class GuildBehaviorRestrictionExtensions
{
    /// <summary>
    ///     修改服务器的行为限制信息。
    /// </summary>
    /// <param name="restriction"> 要修改的服务器行为限制信息。 </param>
    /// <param name="func"> 用于修改行为限制属性的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <exception cref="NotSupportedException">
    ///     当 <paramref name="restriction"/> 不是 <see cref="Kook.Rest.RestGuildBehaviorRestriction"/> 时引发。
    /// </exception>
    public static async Task ModifyAsync(this IGuildBehaviorRestriction restriction,
        Action<ModifyGuildBehaviorRestrictionProperties> func, RequestOptions? options = null)
    {
        if (restriction is RestGuildBehaviorRestriction restGuildBehaviorRestriction)
            await RestGuildBehaviorRestrictionExtensions.ModifyAsync(restGuildBehaviorRestriction, func, options);
        throw new NotSupportedException("Experimental guild behavior restriction extensions are only supported for RestGuildBehaviorRestriction.");
    }
}
