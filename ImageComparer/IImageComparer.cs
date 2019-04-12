using System.Collections.Generic;
using System.Drawing;

namespace ImageComparer
{
    public interface IImageComparer
    {
        int Threshold { get; }
        
        SortedSet<Point> GetDifferences(Image left, Image right);
    }
}