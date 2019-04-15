using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ImageComparerTests
{
    public static class IAsyncEnumerableHelper
    {
        public static async Task<IList<T>> ToListAsync<T>(this IAsyncEnumerable<T> asyncEnumerable)
        {
            var result = new Collection<T>();
            await foreach (var element in asyncEnumerable) result.Add(element);

            return result;
        }
    }
}