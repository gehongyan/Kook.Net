namespace KaiHeiLa;

// The identifier for intimacy is the associated user's id.
public interface IIntimacy : IEntity<ulong>
{
    IUser User { get; }

    string SocialInfo { get; }

    DateTimeOffset LastReadAt { get; }
    
    DateTimeOffset LastModifyAt { get; }

    int Score { get; }
    
    IReadOnlyCollection<IntimacyImage> Images { get; }

    /// <summary>
    ///     Updates the intimacy information with this user.
    /// </summary>
    /// <param name="func">A delegate containing the properties to modify the <see cref="IIntimacy"/> with.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>A task that represents the asynchronous operation for updating the intimacy information.</returns>
    Task UpdateAsync(Action<IntimacyProperties> func, RequestOptions options);
}