namespace Kook;

/// <summary>
///     A meta data containing boost subscription information.
/// </summary>
public class BoostSubscriptionMetadata
{
    public DateTimeOffset Since { get; private set; }

    public DateTimeOffset Until { get; private set; }

    public int Count { get; private set; }

    internal BoostSubscriptionMetadata(DateTimeOffset since, DateTimeOffset until, int count)
    {
        Since = since;
        Until = until;
        Count = count;
    }
}
