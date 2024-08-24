namespace Kook.Commands;

/// <summary>
///     提供用于 <see cref="T:System.Collections.Generic.IEnumerable`1"/> 的扩展方法。
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    ///     生成两个集合中所有可能的元素组合，并将指定的函数应用于每个组合以生成结果。
    /// </summary>
    /// <typeparam name="TFirst"> 第一个集合中的元素类型。 </typeparam>
    /// <typeparam name="TSecond"> 第二个集合中的元素类型。 </typeparam>
    /// <typeparam name="TResult"> 结果的类型。 </typeparam>
    /// <param name="set"> 用于生成组合的第一个元素集合。 </param>
    /// <param name="others"> 用于生成组合的第二个元素集合。 </param>
    /// <param name="func"> 用于生成结果的函数。 </param>
    /// <returns> 所有可能的元素组合的结果。 </returns>
    public static IEnumerable<TResult> Permutate<TFirst, TSecond, TResult>(
        this IEnumerable<TFirst> set,
        IEnumerable<TSecond> others,
        Func<TFirst, TSecond, TResult> func) =>
        from elem in set
        from elem2 in others
        select func(elem, elem2);
}
