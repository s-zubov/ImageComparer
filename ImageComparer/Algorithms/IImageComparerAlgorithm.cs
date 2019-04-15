using System.Collections.Generic;
using System.Drawing;

namespace ImageComparer.Algorithms
{
    public interface IImageComparerAlgorithm
    {
        IEnumerable<RectangleF> GetDifferences(Bitmap left, Bitmap right, int threshold);

        IAsyncEnumerable<RectangleF> GetDifferencesAsync(Bitmap left, Bitmap right, int threshold, object lockObject);
    }
}