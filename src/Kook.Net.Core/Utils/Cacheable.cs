using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Kook;

/// <summary>
///     Represents a cached entity.
/// </summary>
/// <typeparam name="TEntity">The type of entity that is cached.</typeparam>
/// <typeparam name="TId">The type of this entity's ID.</typeparam>
#if DEBUG
[DebuggerDisplay("{DebuggerDisplay,nq}")]
#endif
public readonly struct Cacheable<TEntity, TId>
    where TEntity : IEntity<TId>
    where TId : IEquatable<TId>
{
    /// <summary>
    ///     Gets whether this entity is cached.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue { get; }

    /// <summary>
    ///     Gets the ID of this entity.
    /// </summary>
    public TId Id { get; }

    /// <summary>
    ///     Gets the entity if it could be pulled from cache.
    /// </summary>
    /// <remarks>
    ///     This value is not guaranteed to be set; in cases where the entity cannot be pulled from cache, it is
    ///     <c>null</c>.
    /// </remarks>
    public TEntity? Value { get; }

    private Func<Task<TEntity?>> DownloadFunc { get; }

    internal Cacheable(TEntity? value, TId id, bool hasValue, Func<Task<TEntity?>> downloadFunc)
    {
        Value = value;
        Id = id;
        HasValue = hasValue;
        DownloadFunc = downloadFunc;
    }

    /// <summary>
    ///     Downloads this entity to cache.
    /// </summary>
    /// <exception cref="Kook.Net.HttpException">Thrown when used from a user account.</exception>
    /// <exception cref="NullReferenceException">Thrown when the entity is deleted.</exception>
    /// <returns>
    ///     A task that represents the asynchronous download operation. The task result contains
    ///     the downloaded entity.
    /// </returns>
    public async Task<TEntity?> DownloadAsync() => await DownloadFunc().ConfigureAwait(false);

    /// <summary>
    ///     Returns the cached entity if it exists; otherwise downloads it.
    /// </summary>
    /// <exception cref="Kook.Net.HttpException">Thrown when used from a user account.</exception>
    /// <exception cref="NullReferenceException">Thrown when the entity is deleted and is not in cache.</exception>
    /// <returns>
    ///     A task that represents the asynchronous operation that attempts to get the entity via cache or to
    ///     download the entity. The task result contains the downloaded entity.
    /// </returns>
    public async Task<TEntity?> GetOrDownloadAsync() => HasValue ? Value : await DownloadAsync().ConfigureAwait(false);

#if DEBUG
    private string DebuggerDisplay => HasValue && Value != null
        ? $"{Value.GetType().GetProperty("DebuggerDisplay", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(Value) ?? Value.ToString()} (Cacheable)"
        : $"{Id} (Cacheable, {typeof(TEntity).Name})";
#endif
}

/// <summary>
///     Represents a cached entity that can be downloaded.
/// </summary>
/// <typeparam name="TCachedEntity"> The type of entity that is cached. </typeparam>
/// <typeparam name="TDownloadableEntity"> The type of entity that can be downloaded. </typeparam>
/// <typeparam name="TRelationship"> The common type of <typeparamref name="TCachedEntity" /> and <typeparamref name="TDownloadableEntity" />. </typeparam>
/// <typeparam name="TId"> The type of the corresponding entity's ID. </typeparam>
#if DEBUG
[DebuggerDisplay("{DebuggerDisplay,nq}")]
#endif
public readonly struct Cacheable<TCachedEntity, TDownloadableEntity, TRelationship, TId>
    where TCachedEntity : IEntity<TId>, TRelationship
    where TDownloadableEntity : IEntity<TId>, TRelationship
    where TId : IEquatable<TId>
{
    /// <summary>
    ///     Gets whether this entity is cached.
    /// </summary>
    public bool HasValue { get; }

    /// <summary>
    ///     Gets the ID of this entity.
    /// </summary>
    public TId Id { get; }

    /// <summary>
    ///     Gets the entity if it could be pulled from cache.
    /// </summary>
    /// <remarks>
    ///     This value is not guaranteed to be set; in cases where the entity cannot be pulled from cache, it is
    ///     <c>null</c>.
    /// </remarks>
    public TCachedEntity? Value { get; }

    private Func<Task<TDownloadableEntity?>> DownloadFunc { get; }

    internal Cacheable(TCachedEntity? value, TId id, bool hasValue, Func<Task<TDownloadableEntity?>> downloadFunc)
    {
        Value = value;
        Id = id;
        HasValue = hasValue;
        DownloadFunc = downloadFunc;
    }

    /// <summary>
    ///     Downloads this entity.
    /// </summary>
    /// <exception cref="Kook.Net.HttpException">Thrown when used from a user account.</exception>
    /// <exception cref="NullReferenceException">Thrown when the entity is deleted.</exception>
    /// <returns>
    ///     A task that represents the asynchronous download operation. The task result contains the downloaded
    ///     entity.
    /// </returns>
    public async Task<TDownloadableEntity?> DownloadAsync() => await DownloadFunc().ConfigureAwait(false);

    /// <summary>
    ///     Returns the cached entity if it exists; otherwise downloads it.
    /// </summary>
    /// <exception cref="Kook.Net.HttpException">Thrown when used from a user account.</exception>
    /// <exception cref="NullReferenceException">Thrown when the entity is deleted and is not in cache.</exception>
    /// <returns>
    ///     A task that represents the asynchronous operation that attempts to get the entity via cache or to
    ///     download the entity. The task result contains the downloaded entity.
    /// </returns>
    public async Task<TRelationship?> GetOrDownloadAsync() => HasValue ? Value : await DownloadAsync().ConfigureAwait(false);

#if DEBUG
    private string DebuggerDisplay => HasValue && Value != null
        ? $"{Value.GetType().GetProperty("DebuggerDisplay", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(Value) ?? Value.ToString()} (Cacheable)"
        : $"{Id} (Cacheable, {typeof(TRelationship).Name})";
#endif
}
