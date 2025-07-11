namespace Kook;

/// <summary>
///     查询帖子列表时的排序方式。
/// </summary>
public enum ThreadSortMode
{
    /// <summary>
    ///     按照频道默认的排序方式进行排序。
    /// </summary>
    Inherited = 0,

    /// <summary>
    ///     按照最后活动时间倒序排列。
    /// </summary>
    LatestActive = 1,

    /// <summary>
    ///     按照创建时间倒序排列。
    /// </summary>
    LatestCreated = 2,
}
