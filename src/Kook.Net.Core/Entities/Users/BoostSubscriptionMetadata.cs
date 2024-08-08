namespace Kook;

/// <summary>
///     表示一个包含服务器助力订阅信息的元数据。
/// </summary>
public class BoostSubscriptionMetadata
{
    /// <summary>
    ///     获取此订阅开始的日期和时间。
    /// </summary>
    public DateTimeOffset Since { get; private set; }

    /// <summary>
    ///     获取此订阅将于或已于何时结束的日期和时间。
    /// </summary>
    public DateTimeOffset Until { get; private set; }

    /// <summary>
    ///     获取此订阅是否仍在有效期内。
    /// </summary>
    public bool IsValid => DateTimeOffset.Now < Until;

    /// <summary>
    ///     获取用户为此订阅所使用的助力包数量。
    /// </summary>
    public int Count { get; private set; }

    internal BoostSubscriptionMetadata(DateTimeOffset since, DateTimeOffset until, int count)
    {
        Since = since;
        Until = until;
        Count = count;
    }
}
