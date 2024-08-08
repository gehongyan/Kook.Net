namespace Kook;

/// <summary>
///     表示实体对象可以被删除。
/// </summary>
public interface IDeletable
{
    /// <summary>
    ///     删除此对实体象及其所有子实体对象。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    Task DeleteAsync(RequestOptions? options = null);
}
