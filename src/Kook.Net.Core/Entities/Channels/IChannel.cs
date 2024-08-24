namespace Kook;

/// <summary>
///     表示一个通用的频道。
/// </summary>
public interface IChannel : IEntity<ulong>
{
    #region General

    /// <summary>
    ///     获取此频道的名称。
    /// </summary>
    string Name { get; }

    #endregion

    #region Users

    /// <summary>
    ///     获取能够查看频道或当前在该频道中的所有用户。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="M:Kook.AsyncEnumerableExtensions.FlattenAsync``1(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{``0}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取所有能够查看该频道或当前在该频道中的用户。此方法会根据 <see cref="F:Kook.KookConfig.MaxUsersPerBatch"/>
    ///     将请求拆分。换句话说，如果有 3000 名用户，而 <see cref="F:Kook.KookConfig.MaxUsersPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 60 个单独请求，因此异步枚举器会异步枚举返回 60 个响应。
    ///     <see cref="M:Kook.AsyncEnumerableExtensions.FlattenAsync``1(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{``0}})" />
    ///     方法可以展开这 60 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的用户集合的异步可枚举对象。 </returns>
    IAsyncEnumerable<IReadOnlyCollection<IUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取此频道中的用户。
    /// </summary>
    /// <param name="id"> 要获取的用户的 ID。 </param>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务结果包含表示找到的用户；如果没有找到，则返回 <c>null</c>。 </returns>
    Task<IUser?> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    #endregion
}
