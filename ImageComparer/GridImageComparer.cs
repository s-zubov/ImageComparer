using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageComparer
{
    public class GridImageComparer : IImageComparer
    {
        private readonly IImageComparer _pixelByPixelImageComparer;

        public GridImageComparer(IImageComparer pixelByPixelImageComparer, int threshold, int xSplit, int ySplit)
        {
            _pixelByPixelImageComparer = pixelByPixelImageComparer;
            XSplit = xSplit;
            YSplit = ySplit;
        }

        public int XSplit { get; }

        public int YSplit { get; }

        public int Threshold => _pixelByPixelImageComparer.Threshold;

        public IEnumerable<RectangleF> GetDifferences(Image left, Image right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            if (right == null)
                throw new ArgumentNullException(nameof(right));

            if (left.Size != right.Size)
                throw new ArgumentException("Source images must be of the same size.");

            var xTransformFactor = (float) left.Width / XSplit;
            var yTransformFactor = (float) left.Height / YSplit;

            var hashLeft = new Bitmap(left, new Size(XSplit, YSplit));
            var hashRight = new Bitmap(right, new Size(XSplit, YSplit));

            return _pixelByPixelImageComparer.GetDifferences(hashLeft, hashRight)
                .Select(p => ScaleRectangle(p, xTransformFactor, yTransformFactor));
        }

        public async IAsyncEnumerable<RectangleF> GetDifferencesAsync(Image left, Image right, object lockObject)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            if (right == null)
                throw new ArgumentNullException(nameof(right));

            float xTransformFactor;
            float yTransformFactor;
            Bitmap hashLeft;
            Bitmap hashRight;
            lock (lockObject)
            {
                if (left.Size != right.Size)
                    throw new ArgumentException("Source images must be of the same size.");

                xTransformFactor = (float) left.Width / XSplit;
                yTransformFactor = (float) left.Height / YSplit;

                hashLeft = new Bitmap(left, new Size(XSplit, YSplit));
                hashRight = new Bitmap(right, new Size(XSplit, YSplit));
            }

            var differences = _pixelByPixelImageComparer.GetDifferencesAsync(hashLeft, hashRight, lockObject);

            await foreach (var rectangle in differences)
                yield return ScaleRectangle(rectangle, xTransformFactor, yTransformFactor);
        }

        private static RectangleF ScaleRectangle(RectangleF rectangle, float xTransformFactor, float yTransformFactor)
        {
            return new RectangleF(rectangle.X * xTransformFactor, rectangle.Y * yTransformFactor,
                rectangle.Width * xTransformFactor, rectangle.Height * yTransformFactor);
        }
    }
}