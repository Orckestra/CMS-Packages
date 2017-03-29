using System.Collections.Generic;

namespace Orckestra.Search.LuceneNET
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> ChunksOf<T>(this IEnumerable<T> sequence, int size)
        {
            using (var enumerator = sequence.GetEnumerator())
                while (enumerator.MoveNext())
                    yield return BatchElements(enumerator, size);
        }

        private static IEnumerable<T> BatchElements<T>(IEnumerator<T> sequence, int size)
        {
            yield return sequence.Current;
            for (int i = 0; i < size-1 && sequence.MoveNext(); i++)
                yield return sequence.Current;
        }
    }
}
