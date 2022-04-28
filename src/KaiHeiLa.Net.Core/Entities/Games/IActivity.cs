namespace KaiHeiLa;

/// <summary>
///     A user's activity status, typically an <see cref="IGame"/>.
/// </summary>
public interface IActivity
{
    /// <summary>
    ///     Gets the type of the activity.
    /// </summary>
    /// <returns>
    ///     The type of activity.
    /// </returns>
    ActivityType Type { get; }
}