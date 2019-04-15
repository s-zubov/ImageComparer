using System;
using System.Collections.Generic;
using System.Drawing;

namespace ImageComparer.Algorithms
{
    public class PixelByPixelImageComparerAlgorithm : IImageComparerAlgorithm
    {
        private readonly IPixelComparerAlgorithm _pixelComparerAlgorithm;

        public PixelByPixelImageComparerAlgorithm(IPixelComparerAlgorithm pixelComparerAlgorithm)
        {
            _pixelComparerAlgorithm = pixelComparerAlgorithm;
        }

        public IEnumerable<RectangleF> GetDifferences(Bitmap left, Bitmap right, int threshold)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            if (right == null)
                throw new ArgumentNullException(nameof(right));

            if (left.Size != right.Size)
                throw new ArgumentException("Source images must be of the same size.");

            for (var x = 0; x < left.Width; x++)
            for (var y = 0; y < left.Height; y++)
                if (!_pixelComparerAlgorithm.PixelEquals(left.GetPixel(x, y), right.GetPixel(x, y), threshold))
                    yield return new Rectangle(x, y, 1, 1);
        }

        public async IAsyncEnumerable<RectangleF> GetDifferencesAsync(Bitmap left, Bitmap right, int threshold,
            object lockObject)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            if (right == null)
                throw new ArgumentNullException(nameof(right));

            lock (lockObject)
            {
                if (left.Size != right.Size)
                    throw new ArgumentException("Source images must be of the same size.");

                for (var x = 0; x < left.Width; x++)
                for (var y = 0; y < left.Height; y++)
                    if (!_pixelComparerAlgorithm.PixelEquals(left.GetPixel(x, y), right.GetPixel(x, y), threshold))
                        yield return new Rectangle(x, y, 1, 1);
            }
        }
    }
}