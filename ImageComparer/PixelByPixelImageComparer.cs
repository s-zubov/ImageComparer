using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace ImageComparer
{
    public class PixelByPixelImageComparer : IImageComparer
    {
        private readonly IPixelComparer _pixelComparer;

        public PixelByPixelImageComparer(IPixelComparer pixelComparer, int threshold)
        {
            _pixelComparer = pixelComparer;
            Threshold = threshold;
        }

        public int Threshold { get; }

        public IEnumerable<RectangleF> GetDifferences(Image left, Image right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            if (right == null)
                throw new ArgumentNullException(nameof(right));

            if (left.Size != right.Size)
                throw new ArgumentException("Source images must be of the same size.");

            var bmLeft = left as Bitmap;
            var bmRight = right as Bitmap;

            for (var x = 0; x < bmLeft.Width; x++)
            for (var y = 0; y < bmLeft.Height; y++)
                if (!_pixelComparer.PixelEquals(bmLeft.GetPixel(x, y), bmRight.GetPixel(x, y), Threshold))
                    yield return new Rectangle(x, y, 1, 1);
        }

        public async IAsyncEnumerable<RectangleF> GetDifferencesAsync(Image left, Image right, object lockObject)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            if (right == null)
                throw new ArgumentNullException(nameof(right));

            lock (lockObject)
            {
                if (left.Size != right.Size)
                    throw new ArgumentException("Source images must be of the same size.");

                var bmLeft = left as Bitmap;
                var bmRight = right as Bitmap;

                for (var x = 0; x < bmLeft.Width; x++)
                for (var y = 0; y < bmLeft.Height; y++)
                    if (!_pixelComparer.PixelEquals(bmLeft.GetPixel(x, y), bmRight.GetPixel(x, y), Threshold))
                        yield return new Rectangle(x, y, 1, 1);
            }
        }
    }
}