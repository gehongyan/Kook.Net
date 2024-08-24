namespace Kook;

/// <summary>
///     表示一个可以被更新的实体对象。
/// </summary>
/// <remarks>
///     更新操作表示的是从 KOOK REST API 获取最新数据并替换当前对象的属性，而非修改 KOOK 服务端的数据。
/// </remarks>
public interface IUpdateable
{
    /// <summary>
    ///     通过 REST API 获取此实体对象的最新状态，并替换当前对象的属性。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步更新操作的任务。 </returns>
    /// <remarks>
    ///     <note type="warning">
    ///         此方法将从 KOOK REST API 获取最新数据，并替换当前对象的属性，而非使用指定的属性修改 KOOK 服务端的数据。
    ///     </note>
    /// </remarks>
    Task UpdateAsync(RequestOptions? options = null);
}
