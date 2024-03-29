using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageComparer.Algorithms
{
    public class GridImageComparerAlgorithm : IImageComparerAlgorithm
    {
        private readonly IImageComparerAlgorithm _innerImageComparerAlgorithm;

        public GridImageComparerAlgorithm(IImageComparerAlgorithm innerImageComparerAlgorithm, int xSplit, int ySplit)
        {
            _innerImageComparerAlgorithm = innerImageComparerAlgorithm;
            XSplit = xSplit;
            YSplit = ySplit;
        }

        public int XSplit { get; }

        public int YSplit { get; }

        public IEnumerable<RectangleF> GetDifferences(Bitmap left, Bitmap right, int threshold)
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

            return _innerImageComparerAlgorithm.GetDifferences(hashLeft, hashRight, threshold)
                .Select(p => ScaleRectangle(p, xTransformFactor, yTransformFactor));
        }

        public async IAsyncEnumerable<RectangleF> GetDifferencesAsync(Bitmap left, Bitmap right, int threshold,
            object lockObject)
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

            var differences =
                _innerImageComparerAlgorithm.GetDifferencesAsync(hashLeft, hashRight, threshold, lockObject);

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