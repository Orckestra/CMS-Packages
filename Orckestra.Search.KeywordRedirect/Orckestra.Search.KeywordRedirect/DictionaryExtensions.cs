using System;
using System.Collections.Concurrent;


namespace Orckestra.Search.KeywordRedirect
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this ConcurrentDictionary<TKey, Lazy<TValue>> dictionary, TKey key, Func<TValue> factory)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var lazy = new Lazy<TValue>(factory);
            return dictionary.GetOrAdd(key, lazy).Value;
        }
    };
}