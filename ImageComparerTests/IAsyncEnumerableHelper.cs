using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading.Tasks;

namespace Tests
{
    public static class IAsyncEnumerableHelper
    {
        public static async Task<IList<T>> IterateToListAsync<T>(this IAsyncEnumerable<T> asyncEnumerable)
        {
            var result = new Collection<T>();
            await foreach (var element in asyncEnumerable) result.Add(element);

            return result;
        }
    }
}