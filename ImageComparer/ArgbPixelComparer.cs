using System;
using System.Drawing;

namespace ImageComparer
{
    public class ArgbPixelComparer : IPixelComparer
    {
        public bool PixelEquals(Color left, Color right)
        {
            return PixelEquals(left, right, 0);
        }

        public bool PixelEquals(Color left, Color right, int threshold)
        {
            return Math.Sqrt(Math.Pow(left.R - right.R, 2) + Math.Pow(left.G - right.G, 2) +
                             Math.Pow(left.B - right.B, 2)) + Math.Abs(left.A - right.A) <= threshold;
        }
    }
}