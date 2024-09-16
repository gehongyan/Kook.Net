namespace Kook;

/// <summary>
///     提供用于展开其异步可枚举成员是嵌套的 <see cref="System.Collections.Generic.IEnumerable{T}"/> 可枚举对象的
///     <see cref="System.Collections.Generic.IAsyncEnumerable{T}"/> 的扩展方法。
/// </summary>
public static class AsyncEnumerableExtensions
{
    /// <inheritdoc cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    public static async Task<IEnumerable<T>> FlattenAsync<T>(this IAsyncEnumerable<IEnumerable<T>> source) =>
        await source.Flatten().ToArrayAsync().ConfigureAwait(false);

    /// <summary>
    ///     获取异步可枚举对象 <paramref name="source"/> 的所有 <see cref="System.Collections.Generic.IEnumerable{T}"/>
    ///     成员，并将这些可枚举对象 <see cref="System.Collections.Generic.IEnumerable{T}"/> 中的所有成员迭代返回为一个
    ///     <see cref="System.Collections.Generic.IEnumerable{T}"/>。
    /// </summary>
    public static IAsyncEnumerable<T> Flatten<T>(this IAsyncEnumerable<IEnumerable<T>> source) =>
        source.SelectMany(enumerable => enumerable.ToAsyncEnumerable());
}
