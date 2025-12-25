using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Kook;

/// <summary>
///     表示一个可延迟加载的缓存实体。
/// </summary>
/// <typeparam name="TEntity"> 可延迟加载的缓存实体的类型。 </typeparam>
/// <typeparam name="TId"> 可延迟加载的缓存实体的 ID 的类型。 </typeparam>
public readonly struct Cacheable<TEntity, TId>
    where TEntity : IEntity<TId>
    where TId : IEquatable<TId>
{
    /// <summary>
    ///     获取此实体是否已缓存。
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue { get; }

    /// <summary>
    ///     获取此实体的唯一标识符。
    /// </summary>
    public TId Id { get; }

    /// <summary>
    ///     如果可以从缓存中获取实体，则获取该实体。
    /// </summary>
    /// <remarks>
    ///     此属性不保证非空；在无法从缓存中拉取实体的情况下，此属性返回 <c>null</c>。
    /// </remarks>
    public TEntity? Value { get; }

    private Func<Task<TEntity?>> DownloadFunc { get; }

    /// <summary>
    ///     创建一个新的可延迟加载的缓存实体。
    /// </summary>
    /// <param name="value"> 如果可以从缓存中获取实体，则为该实体；否则为 <c>null</c>。 </param>
    /// <param name="id"> 实体的唯一标识符。 </param>
    /// <param name="hasValue"> 指示实体是否已缓存的值。 </param>
    /// <param name="downloadFunc"> 用于下载实体的异步函数。 </param>
    public Cacheable(TEntity? value, TId id, bool hasValue, Func<Task<TEntity?>> downloadFunc)
    {
        Value = value;
        Id = id;
        HasValue = hasValue;
        DownloadFunc = downloadFunc;
    }

    /// <summary>
    ///     将此实体下载到缓存中。
    /// </summary>
    /// <returns>
    ///     一个表示异步下载操作的任务。任务结果包含下载的实体；如果无法通过 API 请求下载实体，或下载的实体无法转换为
    ///     <typeparamref name="TEntity" />，则为 <c>null</c>。
    /// </returns>
    public async Task<TEntity?> DownloadAsync() => await DownloadFunc().ConfigureAwait(false);

    /// <summary>
    ///     如果实体已存在于缓存中，则返回该实体；否则下载该实体并返回。
    /// </summary>
    /// <returns>
    ///     一个表示异步获取或下载操作的任务。任务结果包含所获取或下载的实体；如果无法通过 API 请求下载实体，或下载的实体无法转换为
    ///     <typeparamref name="TEntity" />，则为 <c>null</c>。
    /// </returns>
    public async Task<TEntity?> GetOrDownloadAsync() => HasValue ? Value : await DownloadAsync().ConfigureAwait(false);

}

/// <summary>
///     表示一个可延迟加载的缓存实体。
/// </summary>
/// <typeparam name="TCachedEntity"> 可延迟加载的缓存实体的类型。 </typeparam>
/// <typeparam name="TDownloadableEntity"> 可从 API 请求下载的实体的类型。 </typeparam>
/// <typeparam name="TRelationship"> 由 <typeparamref name="TCachedEntity" /> 和 <typeparamref name="TDownloadableEntity" /> 共同继承或实现的类型。 </typeparam>
/// <typeparam name="TId"> 可延迟加载的缓存实体的 ID 的类型。 </typeparam>
public readonly struct Cacheable<TCachedEntity, TDownloadableEntity, TRelationship, TId>
    where TCachedEntity : IEntity<TId>, TRelationship
    where TDownloadableEntity : IEntity<TId>, TRelationship
    where TId : IEquatable<TId>
{
    /// <summary>
    ///     获取此实体是否已缓存。
    /// </summary>
    public bool HasValue { get; }

    /// <summary>
    ///     获取此实体的唯一标识符。
    /// </summary>
    public TId Id { get; }

    /// <summary>
    ///     如果可以从缓存中获取实体，则获取该实体。
    /// </summary>
    /// <remarks>
    ///     此属性不保证非空；在无法从缓存中拉取实体的情况下，此属性返回 <c>null</c>。
    /// </remarks>
    public TCachedEntity? Value { get; }

    private Func<Task<TDownloadableEntity?>> DownloadFunc { get; }

    /// <summary>
    ///     创建一个新的可延迟加载的缓存实体。
    /// </summary>
    /// <param name="value"> 如果可以从缓存中获取实体，则为该实体；否则为 <c>null</c>。 </param>
    /// <param name="id"> 实体的唯一标识符。 </param>
    /// <param name="hasValue"> 指示实体是否已缓存的值。 </param>
    /// <param name="downloadFunc"> 用于下载实体的异步函数。 </param>
    public Cacheable(TCachedEntity? value, TId id, bool hasValue, Func<Task<TDownloadableEntity?>> downloadFunc)
    {
        Value = value;
        Id = id;
        HasValue = hasValue;
        DownloadFunc = downloadFunc;
    }

    /// <summary>
    ///     将此实体下载到缓存中。
    /// </summary>
    /// <returns>
    ///     一个表示异步下载操作的任务。任务结果包含下载的实体；如果无法通过 API 请求下载实体，或下载的实体无法转换为
    ///     <typeparamref name="TDownloadableEntity" />，则为 <c>null</c>。
    /// </returns>
    public async Task<TDownloadableEntity?> DownloadAsync() => await DownloadFunc().ConfigureAwait(false);

    /// <summary>
    ///     如果实体已存在于缓存中，则返回该实体；否则下载该实体并返回。
    /// </summary>
    /// <returns>
    ///     一个表示异步获取或下载操作的任务。任务结果包含所获取或下载的实体；如果无法通过 API 请求下载实体，或下载的实体无法转换为
    ///     <typeparamref name="TDownloadableEntity" />，则为 <c>null</c>。
    /// </returns>
    public async Task<TRelationship?> GetOrDownloadAsync() => HasValue ? Value : await DownloadAsync().ConfigureAwait(false);

}
