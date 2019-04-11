using System;
using System.Drawing;
using ImageComparer;
using NUnit.Framework;

namespace Tests
{
    public class ArgbPixelByPixelImageComparerIntegrationTests
    {
        private PixelByPixelImageComparer _comparer;
        private Random _rnd;

        [SetUp]
        public void Setup()
        {
            _comparer = new PixelByPixelImageComparer(new ArgbPixelComparer(), 3);
            _rnd = new Random(1);
        }

        private static Image CreateSolidRectangle(int width, int height, Color color)
        {
            var image = new Bitmap(width, height);
            using (var gfx = Graphics.FromImage(image))
            using (var brush = new SolidBrush(color))
            {
                gfx.FillRectangle(brush, 0, 0, width, height);
            }

            return image;
        }

        [Test]
        public void Compare_EqualSolidSource_ReturnsNoDifferences()
        {
            var cyanRect = CreateSolidRectangle(100, 100, Color.Cyan);

            var result = _comparer.Compare(cyanRect, cyanRect);

            var bmResult = result as Bitmap;

            for (var x = 0; x < bmResult.Width; x++)
            for (var y = 0; y < bmResult.Height; y++)
                Assert.AreEqual(_comparer.BackgroundColor.ToArgb(), bmResult.GetPixel(x, y).ToArgb());
        }

        [Test]
        public void Compare_DifferentByAlphaGreaterThanThresholdSolidSource_ReturnsDifferences()
        {
            var leftColor = Color.FromArgb(1, 100, 100, 100);
            var rightColor = Color.FromArgb(10, 100, 100, 100);

            var leftRect = CreateSolidRectangle(100, 100, leftColor);
            var rightRect = CreateSolidRectangle(100, 100, rightColor);

            var result = _comparer.Compare(leftRect, rightRect);

            var bmResult = result as Bitmap;

            for (var x = 0; x < bmResult.Width; x++)
            for (var y = 0; y < bmResult.Height; y++)
                Assert.AreEqual(_comparer.DifferenceColor.ToArgb(), bmResult.GetPixel(x, y).ToArgb());
        }

        [Test]
        public void Compare_DifferentByRgbGreaterThanThresholdSolidSource_ReturnsDifferences()
        {
            var leftColor = Color.FromArgb(1, 100, 100, 100);
            var rightColor = Color.FromArgb(1, 110, 110, 110);

            var leftRect = CreateSolidRectangle(100, 100, leftColor);
            var rightRect = CreateSolidRectangle(100, 100, rightColor);

            var result = _comparer.Compare(leftRect, rightRect);

            var bmResult = result as Bitmap;

            for (var x = 0; x < bmResult.Width; x++)
            for (var y = 0; y < bmResult.Height; y++)
                Assert.AreEqual(_comparer.DifferenceColor.ToArgb(), bmResult.GetPixel(x, y).ToArgb());
        }

        [Test]
        public void Compare_DifferentByArgbGreaterThanThresholdSolidSource_ReturnsDifferenceResult()
        {
            var leftColor = Color.FromArgb(1, 100, 100, 100);
            var rightColor = Color.FromArgb(10, 110, 110, 110);

            var leftRect = CreateSolidRectangle(100, 100, leftColor);
            var rightRect = CreateSolidRectangle(100, 100, rightColor);

            var result = _comparer.Compare(leftRect, rightRect);

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