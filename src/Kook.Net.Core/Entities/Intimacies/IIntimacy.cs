namespace Kook;

/// <summary>
///     Represents a generic intimacy.
/// </summary>
public interface IIntimacy : IEntity<ulong>
{
    /// <summary>
    ///     Gets the user associated with this intimacy.
    /// </summary>
    /// <returns>
    ///     An <see cref="IUser" /> representing the user associated with this intimacy.
    /// </returns>
    IUser User { get; }

    /// <summary>
    ///     Gets the social information associated with this intimacy.
    /// </summary>
    /// <returns>
    ///     A <c>string</c> representing the social information associated with this intimacy.
    /// </returns>
    string SocialInfo { get; }

    /// <summary>
    ///     Gets the time at which the user read the message.
    /// </summary>
    /// <returns>
    ///     A time at which the user read the message.
    /// </returns>
    DateTimeOffset LastReadAt { get; }

    /// <summary>
    ///     Gets the time at which this intimacy was modified last time.
    /// </summary>
    /// <returns>
    ///     A time at which this intimacy was modified last time.
    /// </returns>
    DateTimeOffset LastModifyAt { get; }

    /// <summary>
    ///     Gets the score associated with this intimacy.
    /// </summary>
    /// <returns>
    ///     A <c>int</c> representing the score associated with this intimacy.
    /// </returns>
    int Score { get; }

    /// <summary>
    ///     Gets the images associated with this intimacy.
    /// </summary>
    /// <returns>
    ///     An <see cref="IReadOnlyCollection{IntimacyImage}"/> containing the images associated with this intimacy.
    /// </returns>
    IReadOnlyCollection<IntimacyImage> Images { get; }

    /// <summary>
    ///     Updates the intimacy information with this user.
    /// </summary>
    /// <param name="func">A delegate containing the properties to modify the <see cref="IIntimacy"/> with.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>A task that represents the asynchronous operation for updating the intimacy information.</returns>
    Task UpdateAsync(Action<IntimacyProperties> func, RequestOptions options);
}
