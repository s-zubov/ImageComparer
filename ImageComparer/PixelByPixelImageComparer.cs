using System;
using System.Drawing;

namespace ImageComparer
{
    public class PixelByPixelImageComparer
    {
        private readonly IPixelComparer _pixelComparer;

        public PixelByPixelImageComparer(IPixelComparer pixelComparer, int threshold)
        {
            _pixelComparer = pixelComparer;
            Threshold = threshold;
        }

        public PixelByPixelImageComparer(Color backgroundColor, Color differenceColor, IPixelComparer pixelComparer,
            int threshold)
        {
            BackgroundColor = backgroundColor;
            DifferenceColor = differenceColor;
            _pixelComparer = pixelComparer;
            Threshold = threshold;
        }

        public int Threshold { get; }

        public Color BackgroundColor { get; } = Color.Transparent;

        public Color DifferenceColor { get; } = Color.Red;

        public Image Compare(Image left, Image right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            if (right == null)
                throw new ArgumentNullException(nameof(right));

            if (left.Size != right.Size)
                throw new ArgumentException("Source images must be of the same size.");

            var result = new Bitmap(left.Width, left.Height);

            var bmLeft = left as Bitmap;
            var bmRight = right as Bitmap;

            for (var x = 0; x < bmLeft.Width; x++)
            for (var y = 0; y < bmLeft.Height; y++)
                result.SetPixel(x, y,
                    _pixelComparer.PixelEquals(bmLeft.GetPixel(x, y), bmRight.GetPixel(x, y), Threshold)
                        ? BackgroundColor
                        : DifferenceColor);

            return result;
        }
    }
}