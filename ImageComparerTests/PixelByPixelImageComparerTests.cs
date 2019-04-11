using System;
using System.Drawing;
using ImageComparer;
using Moq;
using NUnit.Framework;

namespace Tests
{
    public class PixelByPixelImageComparerTests
    {
        private PixelByPixelImageComparer _comparer;
        private Mock<IPixelComparer> _pixelComparerMock;

        [SetUp]
        public void Setup()
        {
            _pixelComparerMock = new Mock<IPixelComparer>();
            _comparer = new PixelByPixelImageComparer(_pixelComparerMock.Object, 10);
        }

        [Test]
        public void Compare_LeftIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _comparer.Compare(null, new Bitmap(10, 10)));
        }

        [Test]
        public void Compare_RightIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _comparer.Compare(new Bitmap(10, 10), null));
        }

        [Test]
        public void Compare_SizeOfLeftAndRightIsNotEqual_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _comparer.Compare(new Bitmap(10, 10), new Bitmap(11, 11)));
        }

        [Test]
        public void Compare_SizeOfLeftAndRightIsZero_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _comparer.Compare(new Bitmap(0, 0), new Bitmap(0, 0)));
        }

        [Test]
        public void Compare_CallsPixelComparerForEveryPixel()
        {
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>())).Returns(true);
            _comparer.Compare(new Bitmap(100, 100), new Bitmap(100, 100));

            _pixelComparerMock.Verify(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>()),
                Times.Exactly(100 * 100));
        }

        [Test]
        public void Compare_PixelComparerReturnsTrue_ReturnsBackgroundResult()
        {
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>(), It.IsAny<int>()))
                .Returns(true);

            var result = _comparer.Compare(new Bitmap(100, 100), new Bitmap(100, 100));

            var bmResult = result as Bitmap;

            for (var x = 0; x < bmResult.Width; x++)
            for (var y = 0; y < bmResult.Height; y++)
                Assert.AreEqual(_comparer.BackgroundColor.ToArgb(), bmResult.GetPixel(x, y).ToArgb());
        }

        [Test]
        public void Compare_PixelComparerReturnsFalse_ReturnsDifferenceResult()
        {
            _pixelComparerMock.Setup(p => p.PixelEquals(It.IsAny<Color>(), It.IsAny<Color>())).Returns(false);

            var result = _comparer.Compare(new Bitmap(100, 100), new Bitmap(100, 100));

            var bmResult = result as Bitmap;

            for (var x = 0; x < bmResult.Width; x++)
            for (var y = 0; y < bmResult.Height; y++)
                Assert.AreEqual(_comparer.DifferenceColor.ToArgb(), bmResult.GetPixel(x, y).ToArgb());
        }
    }
}