namespace KaiHeiLa;

/// <summary>
///     频道
/// </summary>
public interface IChannel : IEntity<ulong>
{
    #region General

    /// <summary>
    ///     频道名称
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     创建者 ID
    /// </summary>
    ulong CreateUserId { get; }

    #endregion

    #region Users

    // TODO: GetUsersAsync
    
    /// <summary>
    ///     Gets a user in this channel.
    /// </summary>
    /// <param name="id">The identifier of the user (e.g. <c>168693960628371456</c>).</param>
    /// <param name="mode">The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a user object that
    ///     represents the found user; <c>null</c> if none is found.
    /// </returns>
    Task<IUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

    #endregion
}