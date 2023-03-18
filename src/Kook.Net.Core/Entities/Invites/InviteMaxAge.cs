namespace Kook;

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
    ///     The invite will expire after half an hour (1800 seconds).
    /// </summary>
    _1800 = 1800,

    /// <summary>
    ///     The invite will expire after one hour (3600 seconds).
    /// </summary>
    _3600 = 3600,

    /// <summary>
    ///     The invite will expire after 6 hours (21600 seconds).
    /// </summary>
    _21600 = 21600,

    /// <summary>
    ///     The invite will expire after half a day (43200 seconds).
    /// </summary>
    _43200 = 43200,

    /// <summary>
    ///     The invite will expire after one day (86400 seconds).
    /// </summary>
    _86400 = 86400,

    /// <summary>
    ///     The invite will expire after one week (604800 seconds).
    /// </summary>
    _604800 = 604800
}
