using System;
using System.Collections.ObjectModel;
using System.Drawing;
using ImageComparer;
using Moq;
using NUnit.Framework;

namespace Tests
{
    public class PixelByPixelImageComparerTests
    {
        private IImageComparer _comparer;
        
        private Mock<IPixelComparer> _pixelComparerMock;

        private const int Width = 100;

        private const int Height = 100;

        [SetUp]
        public void Setup()
        {
            _pixelComparerMock = new Mock<IPixelComparer>();
            _comparer = new PixelByPixelImageComparer(_pixelComparerMock.Object, Height);
        }

        [Test]
        public void Compare_LeftIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _comparer.GetDifferences(null, new Bitmap(Width, Height)));
        }

        [Test]
        public void Compare_RightIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _comparer.GetDifferences(new Bitmap(Width, Height), null));
        }

        [Test]
        public void Compare_SizeOfLeftAndRightIsNotEqual_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _comparer.GetDifferences(new Bitmap(Width, Height), new Bitmap(Width + 1, Height + 1)));
        }

        [Test]
        public void Compare_SizeOfLeftAndRightIsZero_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _comparer.GetDifferences(new Bitmap(0, 0), new Bitmap(0, 0)));
        }

        [Test]
        public void Compare_CallsPixelComparerForEveryPixel()
        {
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>())).Returns(true);
            _comparer.GetDifferences(new Bitmap(Width, Height), new Bitmap(Width, Height));

            _pixelComparerMock.Verify(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>()),
                Times.Exactly(Width * Height));
        }

        [Test]
        public void Compare_PixelComparerReturnsTrue_ReturnsEmptySet()
        {
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>()))
                .Returns(true);

            Assert.IsEmpty(_comparer.GetDifferences(new Bitmap(Width, Height), new Bitmap(Width, Height)));
        }

        [Test]
        public void Compare_PixelComparerReturnsFalse_ReturnsAllPoints()
        {
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>())).Returns(false);

            var result = _comparer.GetDifferences(new Bitmap(Width, Height), new Bitmap(Width, Height));
            
            Assert.AreEqual(Width * Height, result.Count);

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                Assert.IsTrue(result.Contains(new Point(x, y)));
        }
        
        [Test]
        public void Compare_PixelComparerReturnsTrueOrFalse_ReturnsAllPointsForFalse()
        {
            var calls = 0;
            const int callsLimit = Width;
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>())).Returns(
                () => ++calls > callsLimit);

            var result = _comparer.GetDifferences(new Bitmap(Width, Height), new Bitmap(Width, Height));
            
            Assert.AreEqual(callsLimit, result.Count);
        }
    }
}