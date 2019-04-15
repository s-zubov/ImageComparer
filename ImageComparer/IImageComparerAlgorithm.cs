using System.Collections.Generic;
using System.Drawing;

namespace ImageComparer
{
    public interface IImageComparerAlgorithm
    {
        int Threshold { get; }
        
        IEnumerable<RectangleF> GetDifferences(Bitmap left, Bitmap right);

        IAsyncEnumerable<RectangleF> GetDifferencesAsync(Bitmap left, Bitmap right, object lockObject);
    }
}