using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        public SortedSet<Point> GetDifferences(Image left, Image right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            if (right == null)
                throw new ArgumentNullException(nameof(right));

            if (left.Size != right.Size)
                throw new ArgumentException("Source images must be of the same size.");

            var bmLeft = left as Bitmap;
            var bmRight = right as Bitmap;
            
            var result = new SortedSet<Point>(new PointComparer());

            for (var x = 0; x < bmLeft.Width; x++)
            for (var y = 0; y < bmLeft.Height; y++)
                if (!_pixelComparer.PixelEquals(bmLeft.GetPixel(x, y), bmRight.GetPixel(x, y), Threshold))
                    result.Add(new Point(x, y));

            return result;
        }
    }
}