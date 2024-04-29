namespace Kook;

/// <summary>
///     A metadata containing boost subscription information.
/// </summary>
public class BoostSubscriptionMetadata
{
    /// <summary>
    ///     Gets the date and time when this subscription began.
    /// </summary>
    public DateTimeOffset Since { get; private set; }

    /// <summary>
    ///     Gets the date and time when this subscription will end or ended.
    /// </summary>
    public DateTimeOffset Until { get; private set; }

    /// <summary>
    ///     Gets whether this subscription has not expired.
    /// </summary>
    public bool IsValid => DateTimeOffset.Now < Until;

    /// <summary>
    ///     Gets how many boost packs the user used for this subscription.
    /// </summary>
    public int Count { get; private set; }

    internal BoostSubscriptionMetadata(DateTimeOffset since, DateTimeOffset until, int count)
    {
        Since = since;
        Until = until;
        Count = count;
    }
}
