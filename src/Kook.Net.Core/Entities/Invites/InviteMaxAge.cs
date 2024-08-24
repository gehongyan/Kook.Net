namespace Kook;

/// <summary>
///     表示一个邀请的最大有效时长。
/// </summary>
public enum InviteMaxAge
{
    /// <summary>
    ///     永不过期。
    /// </summary>
    NeverExpires = 0,

    /// <summary>
    ///     此邀请在创建后半小时（1800 秒）后过期。
    /// </summary>
    _1800 = 1800,

    /// <summary>
    ///     此邀请在创建后一小时（3600 秒）后过期。
    /// </summary>
    _3600 = 3600,

    /// <summary>
    ///     此邀请在创建后 6 小时（21600 秒）后过期。
    /// </summary>
    _21600 = 21600,

    /// <summary>
    ///     此邀请在创建后半天（43200 秒）后过期。
    /// </summary>
    _43200 = 43200,

    /// <summary>
    ///     此邀请在创建后一天（86400 秒）后过期。
    /// </summary>
    _86400 = 86400,

    /// <summary>
    ///     此邀请在创建后一周（604800 秒）后过期。
    /// </summary>
    _604800 = 604800
}
