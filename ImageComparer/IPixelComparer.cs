using System.Drawing;

namespace ImageComparer
{
    public interface IPixelComparer
    {
        bool PixelEquals(Color left, Color right);
        
        bool PixelEquals(Color left, Color right, int threshold);
    }
}