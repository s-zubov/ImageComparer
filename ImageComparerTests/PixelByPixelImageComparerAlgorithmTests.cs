using System;
using System.Drawing;
using System.Linq;
using ImageComparer.Algorithms;
using Moq;
using NUnit.Framework;

namespace ImageComparerTests
{
    public class PixelByPixelImageComparerAlgorithmTests
    {
        private const int Width = 100;

        private const int Height = 100;

        private const int Threshold = 10;

        private IImageComparerAlgorithm _comparerAlgorithm;

        private object _lockObject;

        private Mock<IPixelComparerAlgorithm> _pixelComparerMock;

        [SetUp]
        public void Setup()
        {
            _lockObject = new object();
            _pixelComparerMock = new Mock<IPixelComparerAlgorithm>();
            _comparerAlgorithm = new PixelByPixelImageComparerAlgorithm(_pixelComparerMock.Object);
        }

        [Test]
        public void GetDifferences_LeftIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _comparerAlgorithm.GetDifferences(null, new Bitmap(Width, Height), Threshold).ToList());
        }

        [Test]
        public void GetDifferences_RightIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _comparerAlgorithm.GetDifferences(new Bitmap(Width, Height), null, Threshold).ToList());
        }

        [Test]
        public void GetDifferences_SizeOfLeftAndRightIsNotEqual_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _comparerAlgorithm
                    .GetDifferences(new Bitmap(Width, Height), new Bitmap(Width + 1, Height + 1), Threshold).ToList());
        }

        [Test]
        public void GetDifferences_SizeOfLeftAndRightIsZero_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => _comparerAlgorithm.GetDifferences(new Bitmap(0, 0), new Bitmap(0, 0), Threshold).ToList());
        }

        [Test]
        public void GetDifferences_CallsPixelComparerForEveryPixel()
        {
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>())).Returns(true);
            var _ = _comparerAlgorithm.GetDifferences(new Bitmap(Width, Height), new Bitmap(Width, Height), Threshold)
                .ToList();

            _pixelComparerMock.Verify(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>()),
                Times.Exactly(Width * Height));
        }

        [Test]
        public void GetDifferences_PixelComparerReturnsTrue_ReturnsEmptySet()
        {
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>()))
                .Returns(true);

            Assert.IsEmpty(_comparerAlgorithm
                .GetDifferences(new Bitmap(Width, Height), new Bitmap(Width, Height), Threshold).ToList());
        }

        [Test]
        public void GetDifferences_PixelComparerReturnsFalse_ReturnsAllPoints()
        {
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>()))
                .Returns(false);

            var result = _comparerAlgorithm
                .GetDifferences(new Bitmap(Width, Height), new Bitmap(Width, Height), Threshold).ToList();

            Assert.AreEqual(Width * Height, result.Count);

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                Assert.IsTrue(result.Contains(new Rectangle(x, y, 1, 1)));
        }

        [Test]
        public void GetDifferences_PixelComparerReturnsTrueOrFalse_ReturnsAllPointsForFalse()
        {
            var calls = 0;
            const int callsLimit = (Width + Height) / 2;
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>())).Returns(
                () => ++calls > callsLimit);

            var result = _comparerAlgorithm
                .GetDifferences(new Bitmap(Width, Height), new Bitmap(Width, Height), Threshold).ToList();

            Assert.AreEqual(callsLimit, result.Count);
        }

        [Test]
        public void GetDifferencesAsync_LeftIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _comparerAlgorithm.GetDifferencesAsync(null, new Bitmap(Width, Height), Threshold, _lockObject)
                    .ToListAsync());
        }

        [Test]
        public void GetDifferencesAsync_RightIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _comparerAlgorithm.GetDifferencesAsync(new Bitmap(Width, Height), null, Threshold, _lockObject)
                    .ToListAsync());
        }

        [Test]
        public void GetDifferencesAsync_SizeOfLeftAndRightIsNotEqual_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _comparerAlgorithm.GetDifferencesAsync(new Bitmap(Width, Height),
                    new Bitmap(Width + 1, Height + 1), Threshold, _lockObject).ToListAsync());
        }

        [Test]
        public void GetDifferencesAsync_SizeOfLeftAndRightIsZero_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _comparerAlgorithm.GetDifferencesAsync(new Bitmap(0, 0), new Bitmap(0, 0), Threshold, _lockObject)
                    .ToListAsync());
        }

        [Test]
        public void GetDifferencesAsync_CallsPixelComparerForEveryPixel()
        {
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>())).Returns(true);
            var _ = _comparerAlgorithm
                .GetDifferencesAsync(new Bitmap(Width, Height), new Bitmap(Width, Height), Threshold, _lockObject)
                .ToListAsync()
                .Result;

            _pixelComparerMock.Verify(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>()),
                Times.Exactly(Width * Height));
        }

        [Test]
        public void GetDifferencesAsync_PixelComparerReturnsTrue_ReturnsEmptySet()
        {
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>()))
                .Returns(true);

            Assert.IsEmpty(_comparerAlgorithm
                .GetDifferencesAsync(new Bitmap(Width, Height), new Bitmap(Width, Height), Threshold, _lockObject)
                .ToListAsync()
                .Result);
        }

        [Test]
        public void GetDifferencesAsync_PixelComparerReturnsFalse_ReturnsAllPoints()
        {
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>()))
                .Returns(false);

            var result = _comparerAlgorithm.GetDifferencesAsync(new Bitmap(Width, Height), new Bitmap(Width, Height),
                    Threshold, _lockObject)
                .ToListAsync()
                .Result;

            Assert.AreEqual(Width * Height, result.Count);

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                Assert.IsTrue(result.Contains(new Rectangle(x, y, 1, 1)));
        }

        [Test]
        public void GetDifferencesAsync_PixelComparerReturnsTrueOrFalse_ReturnsAllPointsForFalse()
        {
            var calls = 0;
            const int callsLimit = Width;
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>())).Returns(
                () => ++calls > callsLimit);

            var result = _comparerAlgorithm.GetDifferencesAsync(new Bitmap(Width, Height), new Bitmap(Width, Height),
                    Threshold, _lockObject)
                .ToListAsync()
                .Result;

            Assert.AreEqual(callsLimit, result.Count);
        }
    }
}