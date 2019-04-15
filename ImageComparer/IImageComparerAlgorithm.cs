using System.Collections.Generic;
using System.Drawing;

namespace ImageComparer
{
    public interface IImageComparerAlgorithm
    {
        int Threshold { get; }
        
        IEnumerable<RectangleF> GetDifferences(Image left, Image right);

        IAsyncEnumerable<RectangleF> GetDifferencesAsync(Image left, Image right, object lockObject);
    }
}