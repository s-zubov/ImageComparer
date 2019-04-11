using System;
using System.Drawing;
using ImageComparer;
using NUnit.Framework;

namespace Tests
{
    public class RgbImageComparerTests
    {
        private RgbImageComparer _comparer;
        private Random _rnd;

        [SetUp]
        public void Setup()
        {
            _comparer = new RgbImageComparer();
            _rnd = new Random(1);
        }

        private Image CreateSolidRectangle(int width, int heigth, Color color)
        {
            var image = new Bitmap(width, heigth);
            using (var gfx = Graphics.FromImage(image))
            using (var brush = new SolidBrush(color))
            {
                gfx.FillRectangle(brush, 0, 0, width, heigth);
            }

            return image;
        }

        [Test]
        public void Compare_LeftIsNull_ThrowsNullArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() =>_comparer.Compare(null, new Bitmap(10, 10)));
        }
        
        [Test]
        public void Compare_RightIsNull_ThrowsNullArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => _comparer.Compare(new Bitmap(10, 10), null));
        }
        
        [Test]
        public void Compare_SizeOfLeftAndRightIsNotEqual_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _comparer.Compare(new Bitmap(10,10), new Bitmap(11, 11)));
        }
        
        [Test]
        public void Compare_SizeOfLeftAndRightIsZero_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _comparer.Compare(new Bitmap(0, 0), new Bitmap(0, 0)));
        }

        [Test]
        public void Compare_EqualSolidSource_ReturnsBackgroundResult()
        {
            var cyanRect = CreateSolidRectangle(100, 100, Color.Cyan);

            var result = _comparer.Compare(cyanRect, cyanRect);

            var bmResult = result as Bitmap;

            for (var x = 0; x < bmResult.Width; x++)
            for (var y = 0; y < bmResult.Height; y++)
                Assert.AreEqual(_comparer.BackgroundColor.ToArgb(), bmResult.GetPixel(x, y).ToArgb());
        }

        [Test]
        public void Compare_DifferentSolidSource_ReturnsDifferenceResult()
        {
            var cyanRect = CreateSolidRectangle(100, 100, Color.Cyan);
            var magentaRect = CreateSolidRectangle(100, 100, Color.Magenta);

            var result = _comparer.Compare(cyanRect, magentaRect);

            var bmResult = result as Bitmap;

            for (var x = 0; x < bmResult.Width; x++)
            for (var y = 0; y < bmResult.Height; y++)
                Assert.AreEqual(_comparer.DifferenceColor.ToArgb(), bmResult.GetPixel(x, y).ToArgb());
        }

        [Test]
        public void Compare_DifferentSolidAndRandomSource_ReturnsRandomAsDifference()
        {
            var cyanRect = CreateSolidRectangle(100, 100, Color.Cyan);

            var magentaPixelsOnCyanBg = CreateSolidRectangle(100, 100, Color.Cyan);
            var bmMagentaPixelsOnCyanBg = magentaPixelsOnCyanBg as Bitmap;
            for (var x = 0; x < bmMagentaPixelsOnCyanBg.Width; x++)
            for (var y = 0; y < bmMagentaPixelsOnCyanBg.Height; y++)
                if (_rnd.Next(1, 2) == 1)
                    bmMagentaPixelsOnCyanBg.SetPixel(x, y, Color.Magenta);

            var result = _comparer.Compare(cyanRect, magentaPixelsOnCyanBg);

            var bmResult = result as Bitmap;

            for (var x = 0; x < bmResult.Width; x++)
            for (var y = 0; y < bmResult.Height; y++)
                Assert.AreEqual(
                    bmMagentaPixelsOnCyanBg.GetPixel(x, y).ToArgb().Equals(Color.Magenta.ToArgb())
                        ? _comparer.DifferenceColor.ToArgb()
                        : _comparer.BackgroundColor.ToArgb(), 
                    bmResult.GetPixel(x, y).ToArgb());
        }
    }
}