namespace Kook;

/// <summary>
///     Specifies the number of uses after which an <see cref="IInvite"/> will be expired.
/// </summary>
public enum InviteMaxUses
{
    /// <summary>
    ///     This <see cref="IInvite"/> can be used for unlimited times.
    /// </summary>
    Unlimited = -1,
    /// <summary>
    ///     This <see cref="IInvite"/> can be used only once.
    /// </summary>
    _1 = 1,
    /// <summary>
    ///     This <see cref="IInvite"/> can be used for 5 times.
    /// </summary>
    _5 = 5,
    /// <summary>
    ///     This <see cref="IInvite"/> can be used for 10 times.
    /// </summary>
    _10 = 10,
    /// <summary>
    ///     This <see cref="IInvite"/> can be used for 25 times.
    /// </summary>
    _25 = 25,
    /// <summary>
    ///     This <see cref="IInvite"/> can be used for 50 times.
    /// </summary>
    _50 = 50,
    /// <summary>
    ///     This <see cref="IInvite"/> can be used for 100 times.
    /// </summary>
    _100 = 100
}