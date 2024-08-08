namespace Kook;

/// <summary>
///     提供用于展开其异步可枚举成员是嵌套的 <see cref="T:System.Collections.Generic.IEnumerable`1"/> 可枚举对象的
///     <see cref="T:System.Collections.Generic.IAsyncEnumerable`1"/> 的扩展方法。
/// </summary>
public static class AsyncEnumerableExtensions
{
    /// <inheritdoc cref="M:Kook.AsyncEnumerableExtensions.Flatten``1(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{``0}})" />
    public static async Task<IEnumerable<T>> FlattenAsync<T>(this IAsyncEnumerable<IEnumerable<T>> source) =>
        await source.Flatten().ToArrayAsync().ConfigureAwait(false);

    /// <summary>
    ///     获取异步可枚举对象 <paramref name="source"/> 的所有 <see cref="T:System.Collections.Generic.IEnumerable`1"/>
    ///     成员，并将这些可枚举对象 <see cref="T:System.Collections.Generic.IEnumerable`1"/> 中的所有成员迭代返回为一个
    ///     <see cref="T:System.Collections.Generic.IEnumerable`1"/>。
    /// </summary>
    public static IAsyncEnumerable<T> Flatten<T>(this IAsyncEnumerable<IEnumerable<T>> source) =>
        source.SelectMany(enumerable => enumerable.ToAsyncEnumerable());
}
