using System;
using System.Drawing;
using ImageComparer;
using NUnit.Framework;

namespace Tests
{
    public class ArgbPixelByPixelImageComparerIntegrationTests
    {
        private IImageComparer _comparer;
        
        private Random _rnd;

        private const int Width = 100;

        private const int Height = 100;

        private const int Value = 200;
        
        private const int Threshold = 10;

        private const int GreaterThanThresholdValue = Value + Threshold + 1;

        [SetUp]
        public void Setup()
        {
            _comparer = new PixelByPixelImageComparer(new ArgbPixelComparer(), Threshold);
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
        public void Compare_EqualSolidSource_ReturnsEmptySet()
        {
            var cyanRect = CreateSolidRectangle(Width, Height, Color.Cyan);

            Assert.IsEmpty(_comparer.GetDifferences(cyanRect, cyanRect));
        }

        [Test]
        public void Compare_DifferentByAlphaGreaterThanThresholdSolidSource_ReturnsAllPoints()
        {
            var leftColor = Color.FromArgb(Value, Value, Value, Value);
            var rightColor = Color.FromArgb(GreaterThanThresholdValue, Value, Value, Value);

            var leftRect = CreateSolidRectangle(Width, Height, leftColor);
            var rightRect = CreateSolidRectangle(Width, Height, rightColor);

            var result = _comparer.GetDifferences(leftRect, rightRect);

            Assert.AreEqual(Width * Height, result.Count);
            
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                Assert.IsTrue(result.Contains(new Point(x, y)));;
        }

        [Test]
        public void Compare_DifferentByRgbGreaterThanThresholdSolidSource_ReturnsAllPoints()
        {
            var leftColor = Color.FromArgb(Value, Value, Value, Value);
            var rightColor = Color.FromArgb(Value, GreaterThanThresholdValue, GreaterThanThresholdValue, GreaterThanThresholdValue);

            var leftRect = CreateSolidRectangle(Width, Height, leftColor);
            var rightRect = CreateSolidRectangle(Width, Height, rightColor);

            var result = _comparer.GetDifferences(leftRect, rightRect);

            Assert.AreEqual(Width * Height, result.Count);
            
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                Assert.IsTrue(result.Contains(new Point(x, y)));
        }

        [Test]
        public void Compare_DifferentByArgbGreaterThanThresholdSolidSource_ReturnsAllPoints()
        {
            var leftColor = Color.FromArgb(Value, Value, Value, Value);
            var rightColor = Color.FromArgb(GreaterThanThresholdValue, GreaterThanThresholdValue, GreaterThanThresholdValue, GreaterThanThresholdValue);

            var leftRect = CreateSolidRectangle(Width, Height, leftColor);
            var rightRect = CreateSolidRectangle(Width, Height, rightColor);

            var result = _comparer.GetDifferences(leftRect, rightRect);
            
            Assert.AreEqual(Width * Height, result.Count);
            
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                Assert.IsTrue(result.Contains(new Point(x, y)));
        }

        [Test]
        public void Compare_DifferentSolidAndRandomSource_ReturnsAllRandomPoints()
        {
            var cyanRect = CreateSolidRectangle(Width, Height, Color.Cyan);

            var magentaPixelsOnCyanBg = CreateSolidRectangle(Width, Height, Color.Cyan);
            var bmMagentaPixelsOnCyanBg = magentaPixelsOnCyanBg as Bitmap;
            var randomPointsCount = 0;
            for (var x = 0; x < bmMagentaPixelsOnCyanBg.Width; x++)
            for (var y = 0; y < bmMagentaPixelsOnCyanBg.Height; y++)
                if (_rnd.Next(1, 2) == 1)
                {
                    randomPointsCount++;
                    bmMagentaPixelsOnCyanBg.SetPixel(x, y, Color.Magenta);
                }

            var result = _comparer.GetDifferences(cyanRect, magentaPixelsOnCyanBg);
            
            Assert.AreEqual(randomPointsCount, result.Count);
            
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                Assert.IsTrue(result.Contains(new Point(x, y)));
        }
    }
}