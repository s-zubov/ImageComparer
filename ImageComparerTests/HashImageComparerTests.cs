using System.Drawing;
using System.Drawing.Drawing2D;
using ImageComparer;
using NUnit.Framework;

namespace Tests
{
    public class HashImageComparerTests
    {
        private HashImageComparer _hashImageComparer;
        private PixelByPixelImageComparer _pixelByPixelImageComparer;

        [SetUp]
        public void SetUp()
        {
            var threshold = 20;
            _hashImageComparer = new HashImageComparer(new ArgbPixelComparer(), threshold);
            _pixelByPixelImageComparer = new PixelByPixelImageComparer(new ArgbPixelComparer(), threshold);
        }

        [Test]
        public void Test()
        {
            var left = Image.FromFile("left.jpg");
            var right = Image.FromFile("right.jpg");

            var result1 = _hashImageComparer.GetDifferences(left, right);
            var result2 = _pixelByPixelImageComparer.GetDifferences(left, right);

            var g = Graphics.FromImage(left);
            (result1 as Bitmap).MakeTransparent(Color.Black);
            g.CompositingMode = CompositingMode.SourceOver;
            g.DrawImage(result1, new Point(0, 0));

            left.Save("result1.jpg");
//
//            var g2 = Graphics.FromImage(right);
//            (result2 as Bitmap).MakeTransparent(Color.Black);
//            g2.CompositingMode = CompositingMode.SourceOver;
//            g2.DrawImage(result2, new Point(0, 0));
//            
//            right.Save("result2.jpg");
        }
    }
}