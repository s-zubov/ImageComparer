using System.Drawing;

namespace ImageComparer.Algorithms
{
    public interface IPixelComparerAlgorithm
    {
        bool PixelEquals(Color left, Color right);

        bool PixelEquals(Color left, Color right, int threshold);
    }
}