using Kook.Rest;

namespace Kook.WebSocket;

/// <summary>
///     Provides extension methods of experimental functionalities for <see cref="SocketGuild"/>s.
/// </summary>
public static class SocketGuildExperimentalExtensions
{
    /// <summary>
    ///     Deletes this guild.
    /// </summary>
    /// <param name="guild">The guild to delete.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>A task that represents the asynchronous deletion operation.</returns>
    /// <remarks>
    ///     <note type="warning">
    ///         This method is still in experimental state, which means that it is not for official API implementation
    ///         usage, may violate the developer rules or policies, not guaranteed to be stable, and may be changed or removed in the future.
    ///     </note>
    /// </remarks>
    public static Task DeleteAsync(this SocketGuild guild, RequestOptions? options = null) =>
        ExperimentalGuildHelper.DeleteAsync(guild, guild.Kook, options);

    /// <summary>
    ///     Modifies this guild.
    /// </summary>
    /// <param name="guild">The guild to modify.</param>
    /// <param name="func">The delegate containing the properties to modify the guild with.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous modification operation.
    /// </returns>
    /// <remarks>
    ///     <note type="warning">
    ///         This method is still in experimental state, which means that it is not for official API implementation
    ///         usage, may violate the developer rules or policies, not guaranteed to be stable, and may be changed or removed in the future.
    ///     </note>
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="func"/> is <c>null</c>.</exception>
    public static Task ModifyAsync(this SocketGuild guild, Action<GuildProperties> func, RequestOptions? options = null) =>
        ExperimentalGuildHelper.ModifyAsync(guild, guild.Kook, func, options);
}
