using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OffLeaseOnly
{
    public static class IEnumerableExtension
    {
        public static Dictionary<TKey, int> ToDict<TKey, TElement>(this IEnumerable<IGrouping<TKey, TElement>> source)
        {
            return source.ToDictionary(p => p.Key, p => p.Count());
        }
    }
}