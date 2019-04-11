using System;
using System.Drawing;

namespace ImageComparer
{
    public class RgbImageComparer
    {
        public RgbImageComparer()
        {
        }

        public RgbImageComparer(Color backgroundColor, Color differenceColor)
        {
            BackgroundColor = backgroundColor;
            DifferenceColor = differenceColor;
        }

        public Color BackgroundColor { get; set; } = Color.Transparent;

        public Color DifferenceColor { get; set; } = Color.Red;

        public Image Compare(Image left, Image right)
        {
            if(left == null)
                throw new ArgumentNullException(nameof(left));
            
            if(right == null)
                throw new ArgumentNullException(nameof(right));
            
            if(left.Size != right.Size)
                throw new ArgumentException("Source images must be of the same size.");
            
            var result = new Bitmap(left.Width, left.Height);

            var different = false;

            var bmLeft = left as Bitmap;
            var bmRight = right as Bitmap;
            
            for(var x = 0; x < bmLeft.Width; x++)
                for(var y = 0; y < bmLeft.Height; y++)
                    if(bmLeft.GetPixel(x, y).Equals(bmRight.GetPixel(x, y)))
                        result.SetPixel(x, y, BackgroundColor);
                    else
                    {
                        different = true;
                        result.SetPixel(x, y, DifferenceColor);
                    }

            return result;
        }
    }
}