using System.Drawing;

namespace KaiHeiLa;

public interface IRole : IEntity<uint>, IMentionable
{
    IGuild Guild { get; }
    
    string Name { get; }

    Color Color { get; }

    int Position { get; }

    bool IsHoisted { get; }

    bool IsMentionable { get; }

    GuildPermissions Permissions { get; }
    
    /// <summary>
    ///     Modifies this role.
    /// </summary>
    /// <remarks>
    ///     This method modifies this role with the specified properties. To see an example of this
    ///     method and what properties are available, please refer to <see cref="RoleProperties"/>.
    /// </remarks>
    /// <param name="func">A delegate containing the properties to modify the role with.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous modification operation.
    /// </returns>
    Task ModifyAsync(Action<RoleProperties> func, RequestOptions options = null);

    /// <summary>
    ///     Gets a collection of users with this role.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <returns>
    ///     Paged collection of users with this role.
    /// </returns>
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
}