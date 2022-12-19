using Kook.Rest;

namespace Kook.WebSocket;

public static class SocketGuildExperimentalExtensions
{
    /// <summary>
    ///     Deletes this guild.
    /// </summary>
    /// <param name="guild">The guild to delete.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>A task that represents the asynchronous deletion operation.</returns>
    public static Task DeleteAsync(this SocketGuild guild, RequestOptions options = null)
        => ExperimentalGuildHelper.DeleteAsync(guild, guild.Kook, options);
    /// <summary>
    ///     Modifies this guild.
    /// </summary>
    /// <param name="guild">The guild to modify.</param>
    /// <param name="func">The delegate containing the properties to modify the guild with.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous modification operation.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
    public static Task ModifyAsync(this SocketGuild guild, Action<GuildProperties> func, RequestOptions options = null)
        => ExperimentalGuildHelper.ModifyAsync(guild, guild.Kook, func, options);
}
