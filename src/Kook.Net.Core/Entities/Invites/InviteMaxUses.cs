namespace Kook;

/// <summary>
///     表示一个邀请的可用人次。
/// </summary>
public enum InviteMaxUses
{
    /// <summary>
    ///     此邀请不限制可用人次。
    /// </summary>
    Unlimited = -1,

    /// <summary>
    ///     此邀请最多只能使用一次。
    /// </summary>
    _1 = 1,

    /// <summary>
    ///     此邀请最多只能使用 5 次。
    /// </summary>
    _5 = 5,

    /// <summary>
    ///     此邀请最多只能使用 10 次。
    /// </summary>
    _10 = 10,

    /// <summary>
    ///     此邀请最多只能使用 25 次。
    /// </summary>
    _25 = 25,

    /// <summary>
    ///     此邀请最多只能使用 50 次。
    /// </summary>
    _50 = 50,

    /// <summary>
    ///     此邀请最多只能使用 100 次。
    /// </summary>
    _100 = 100
}
