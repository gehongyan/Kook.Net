namespace KaiHeiLa;

/// <summary>
///     Specifies the time in second after which an <see cref="IInvite"/> will be expired.
/// </summary>
public enum InviteMaxAge
{
    /// <summary>
    ///     The invite will never expire.
    /// </summary>
    NeverExpires = 0,
    /// <summary>
    ///     The invite will expire after half an hour.
    /// </summary>
    HalfAnHour = 1800,
    /// <summary>
    ///     The invite will expire after one hour.
    /// </summary>
    OneHour = 3600,
    /// <summary>
    ///     The invite will expire after 6 hours.
    /// </summary>
    SixHours = 21600,
    /// <summary>
    ///     The invite will expire after half a day.
    /// </summary>
    HalfADay = 43200,
    /// <summary>
    ///     The invite will expire after one day.
    /// </summary>
    OneDay = 86400,
    /// <summary>
    ///     The invite will expire after one week.
    /// </summary>
    OneWeek = 604800
}