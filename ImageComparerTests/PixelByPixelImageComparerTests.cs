using System;
using System.Drawing;
using System.Linq;
using ImageComparer;
using Moq;
using NUnit.Framework;

namespace Tests
{
    public class PixelByPixelImageComparerTests
    {
        private const int Width = 100;

        private const int Height = 100;

        private const int Threshold = 10;

        private IImageComparer _comparer;

        private Mock<IPixelComparer> _pixelComparerMock;

        private object _lockObject;

        [SetUp]
        public void Setup()
        {
            _lockObject = new object();
            _pixelComparerMock = new Mock<IPixelComparer>();
            _comparer = new PixelByPixelImageComparer(_pixelComparerMock.Object, Threshold);
        }

        [Test]
        public void GetDifferences_LeftIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _comparer.GetDifferences(null, new Bitmap(Width, Height)).ToList());
        }

        [Test]
        public void GetDifferences_RightIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _comparer.GetDifferences(new Bitmap(Width, Height), null).ToList());
        }

        [Test]
        public void GetDifferences_SizeOfLeftAndRightIsNotEqual_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _comparer.GetDifferences(new Bitmap(Width, Height), new Bitmap(Width + 1, Height + 1)).ToList());
        }

        [Test]
        public void GetDifferences_SizeOfLeftAndRightIsZero_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => _comparer.GetDifferences(new Bitmap(0, 0), new Bitmap(0, 0)).ToList());
        }

        [Test]
        public void GetDifferences_CallsPixelComparerForEveryPixel()
        {
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>())).Returns(true);
            var _ = _comparer.GetDifferences(new Bitmap(Width, Height), new Bitmap(Width, Height)).ToList();

            _pixelComparerMock.Verify(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>()),
                Times.Exactly(Width * Height));
        }

        [Test]
        public void GetDifferences_PixelComparerReturnsTrue_ReturnsEmptySet()
        {
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>()))
                .Returns(true);

            Assert.IsEmpty(_comparer.GetDifferences(new Bitmap(Width, Height), new Bitmap(Width, Height)).ToList());
        }

        [Test]
        public void GetDifferences_PixelComparerReturnsFalse_ReturnsAllPoints()
        {
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>()))
                .Returns(false);

            var result = _comparer.GetDifferences(new Bitmap(Width, Height), new Bitmap(Width, Height)).ToList();

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

            var result = _comparer.GetDifferences(new Bitmap(Width, Height), new Bitmap(Width, Height)).ToList();

            Assert.AreEqual(callsLimit, result.Count);
        }

        [Test]
        public void GetDifferencesAsync_LeftIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _comparer.GetDifferencesAsync(null, new Bitmap(Width, Height), _lockObject).IterateToListAsync());
        }

        [Test]
        public void GetDifferencesAsync_RightIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _comparer.GetDifferencesAsync(new Bitmap(Width, Height), null, _lockObject).IterateToListAsync());
        }

        [Test]
        public void GetDifferencesAsync_SizeOfLeftAndRightIsNotEqual_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _comparer.GetDifferencesAsync(new Bitmap(Width, Height),
                    new Bitmap(Width + 1, Height + 1), _lockObject).IterateToListAsync());
        }

        [Test]
        public void GetDifferencesAsync_SizeOfLeftAndRightIsZero_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _comparer.GetDifferencesAsync(new Bitmap(0, 0), new Bitmap(0, 0), _lockObject).IterateToListAsync());
        }

        [Test]
        public void GetDifferencesAsync_CallsPixelComparerForEveryPixel()
        {
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>())).Returns(true);
            var _ = _comparer.GetDifferencesAsync(new Bitmap(Width, Height), new Bitmap(Width, Height), _lockObject)
                .IterateToListAsync()
                .Result;

            _pixelComparerMock.Verify(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>()),
                Times.Exactly(Width * Height));
        }

        [Test]
        public void GetDifferencesAsync_PixelComparerReturnsTrue_ReturnsEmptySet()
        {
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>()))
                .Returns(true);

            Assert.IsEmpty(_comparer.GetDifferencesAsync(new Bitmap(Width, Height), new Bitmap(Width, Height), _lockObject)
                .IterateToListAsync()
                .Result);
        }

        [Test]
        public void GetDifferencesAsync_PixelComparerReturnsFalse_ReturnsAllPoints()
        {
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>()))
                .Returns(false);

            var result = _comparer.GetDifferencesAsync(new Bitmap(Width, Height), new Bitmap(Width, Height), _lockObject)
                .IterateToListAsync()
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

            var result = _comparer.GetDifferencesAsync(new Bitmap(Width, Height), new Bitmap(Width, Height), _lockObject)
                .IterateToListAsync()
                .Result;

            Assert.AreEqual(callsLimit, result.Count);
        }
    }
}