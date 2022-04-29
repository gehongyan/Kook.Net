namespace KaiHeiLa;

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
    One = 1,
    /// <summary>
    ///     This <see cref="IInvite"/> can be used for 5 times.
    /// </summary>
    Five = 5,
    /// <summary>
    ///     This <see cref="IInvite"/> can be used for 10 times.
    /// </summary>
    Ten = 10,
    /// <summary>
    ///     This <see cref="IInvite"/> can be used for 25 times.
    /// </summary>
    TwentyFive = 25,
    /// <summary>
    ///     This <see cref="IInvite"/> can be used for 50 times.
    /// </summary>
    Fifty = 50,
    /// <summary>
    ///     This <see cref="IInvite"/> can be used for 100 times.
    /// </summary>
    OneHundred = 100
}