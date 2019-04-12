using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ImageComparer
{
    public class HashImageComparer
    {
        private readonly PixelByPixelImageComparer _pixelByPixelImageComparer;

        public HashImageComparer(IPixelComparer pixelComparer, int threshold)
        {
            _pixelByPixelImageComparer = new PixelByPixelImageComparer(pixelComparer, threshold);
        }

        public int Threshold => _pixelByPixelImageComparer.Threshold;
        
        public Image GetDifferences(Image left, Image right)
        {
            var xSplit = 32;
            var ySplit = 32;
            
            var xTransformFactor = left.Width / xSplit;
            var yTransformFactor = left.Height / ySplit;
            
            var hashLeft = new Bitmap(left, new Size(xSplit, ySplit));
            var hashRight = new Bitmap(right, new Size(xSplit, ySplit));

            var differences = _pixelByPixelImageComparer.GetDifferences(hashLeft, hashRight);

            var result = new Bitmap(left.Width, left.Height);
            if (!differences.Any())
                return new Bitmap(left.Width, left.Height);
            
            using(var graphics = Graphics.FromImage(result))
            using (var pen = new Pen(Color.Red, 4))
            foreach (var point in differences)
            {
                // Top border
                if(!differences.Contains(new Point(point.X, point.Y - 1)))
                    graphics.DrawLine(pen, point.X * xTransformFactor, point.Y * yTransformFactor, (point.X + 1) * xTransformFactor, point.Y * yTransformFactor);
                
                // Right border
                if(!differences.Contains(new Point(point.X + 1, point.Y)))
                    graphics.DrawLine(pen, (point.X + 1) * xTransformFactor, point.Y * yTransformFactor, (point.X + 1) * xTransformFactor, (point.Y + 1) * yTransformFactor);
                
                // Bottom border
                if(!differences.Contains(new Point(point.X, point.Y + 1)))
                    graphics.DrawLine(pen, point.X * xTransformFactor, (point.Y + 1) * yTransformFactor, (point.X + 1) * xTransformFactor, (point.Y + 1) * yTransformFactor);
                
                // Left border
                if(!differences.Contains(new Point(point.X - 1, point.Y)))
                    graphics.DrawLine(pen, point.X * xTransformFactor, point.Y * yTransformFactor, point.X * xTransformFactor, (point.Y + 1) * yTransformFactor);
            }

            return result;
        }
    }
}