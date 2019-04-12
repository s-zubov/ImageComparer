using System.Collections.Generic;
using System.Drawing;

namespace ImageComparer
{
    public class PointComparer : IComparer<Point>
    {
        public int Compare(Point x, Point y)
        {
            var xStep =  x.X.CompareTo(y.X);
            return xStep == 0 ? x.Y.CompareTo(y.Y) : xStep;
        }
    }
}