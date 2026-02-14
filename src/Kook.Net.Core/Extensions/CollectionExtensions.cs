using System.Collections;
using System.Diagnostics;

namespace Kook;

internal static class CollectionExtensions
{
    // extension<TValue>(IReadOnlyCollection<TValue> source)
    // {
    //     public IReadOnlyCollection<TValue> ToReadOnlyCollection()
    //         => new CollectionWrapper<TValue>(source, () => source.Count);
    // }

    extension<TValue>(ICollection<TValue> source)
    {
        public IReadOnlyCollection<TValue> ToReadOnlyCollection() =>
            new CollectionWrapper<TValue>(source, () => source.Count);
    }

    // extension<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> source)
    // {
    //     public IReadOnlyCollection<TValue> ToReadOnlyCollection()
    //         => new CollectionWrapper<TValue>(source.Select(x => x.Value), () => source.Count);
    // }

    extension<TKey, TValue>(IDictionary<TKey, TValue> source)
    {
        public IReadOnlyCollection<TValue> ToReadOnlyCollection() =>
            new CollectionWrapper<TValue>(source.Values, () => source.Count);
    }

    extension<TValue>(IEnumerable<TValue> query)
    {
        public IReadOnlyCollection<TValue> ToReadOnlyCollection<TSource>(IReadOnlyCollection<TSource> source) =>
            new CollectionWrapper<TValue>(query, () => source.Count);

        public IReadOnlyCollection<TValue> ToReadOnlyCollection(Func<int> countFunc) =>
            new CollectionWrapper<TValue>(query, countFunc);

#if NETSTANDARD || NETFRAMEWORK
        public IEnumerable<TValue[]> Chunk(int size)
        {
            if (query is null)
                throw new ArgumentNullException(nameof(query));

            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size));

            return ChunkIterator(query, size);

            static IEnumerable<TValue[]> ChunkIterator(IEnumerable<TValue> source, int size)
            {
                using IEnumerator<TValue> e = source.GetEnumerator();

                // Before allocating anything, make sure there's at least one element.
                if (e.MoveNext())
                {
                    // Now that we know we have at least one item, allocate an initial storage array. This is not
                    // the array we'll yield.  It starts out small in order to avoid significantly overallocating
                    // when the source has many fewer elements than the chunk size.
                    int arraySize = Math.Min(size, 4);
                    int i;
                    do
                    {
                        TValue[]? array = new TValue[arraySize];

                        // Store the first item.
                        array[0] = e.Current;
                        i = 1;

                        if (size != array.Length)
                        {
                            // This is the first chunk. As we fill the array, grow it as needed.
                            for (; i < size && e.MoveNext(); i++)
                            {
                                if (i >= array.Length)
                                {
                                    arraySize = (int)Math.Min((uint)size, 2 * (uint)array.Length);
                                    Array.Resize(ref array, arraySize);
                                }

                                array[i] = e.Current;
                            }
                        }
                        else
                        {
                            // For all but the first chunk, the array will already be correctly sized.
                            // We can just store into it until either it's full or MoveNext returns false.
                            TValue[] local = array; // avoid bounds checks by using cached local (`array` is lifted to iterator object as a field)
                            for (; (uint)i < (uint)local.Length && e.MoveNext(); i++)
                            {
                                local[i] = e.Current;
                            }
                        }

                        if (i != array.Length)
                        {
                            Array.Resize(ref array, i);
                        }

                        yield return array;
                    }
                    while (i >= size && e.MoveNext());
                }
            }
        }
#endif
    }
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal readonly struct CollectionWrapper<TValue>
    (IEnumerable<TValue> query, Func<int> countFunc) : IReadOnlyCollection<TValue>
{
    //It's okay that this count is affected by race conditions - we're wrapping a concurrent collection and that's to be expected
    public int Count => countFunc();

    private string DebuggerDisplay => $"Count = {Count}";

    public IEnumerator<TValue> GetEnumerator() => query.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => query.GetEnumerator();
}
