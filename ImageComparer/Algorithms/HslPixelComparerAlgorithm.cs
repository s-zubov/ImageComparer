using System;
using System.Drawing;

namespace ImageComparer.Algorithms
{
    public class HsvPixelComparerAlgorithm : IPixelComparerAlgorithm
    {
        public bool PixelEquals(Color left, Color right)
        {
            return PixelEquals(left, right, 0);
        }

        public bool PixelEquals(Color left, Color right, int threshold)
        {
            return Math.Sqrt(Math.Pow(left.GetHue() - right.GetHue(), 2) + Math.Pow(left.GetSaturation() - right.GetSaturation(), 2) +
                             Math.Pow(left.GetBrightness() - right.GetBrightness(), 2)) <= threshold;
        }
    }
}