using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageComparer;
using Moq;
using NUnit.Framework;

namespace Tests
{
    public class ImagePainterTests
    {
        private IImagePainter _imagePainter;

        [SetUp]
        public void SetUp()
        {
            _imagePainter = new ImagePainter(_color, Width);
            _image = new Bitmap(100, 100);
            _lockObject = new object();
        }

        private readonly Color _color = Color.Red;

        private const int Width = 1;

        private Bitmap _image;

        private object _lockObject;

        [Test]
        public void DrawDifferences_ImageIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _imagePainter.DrawDifferences(null, Enumerable.Empty<RectangleF>()));
        }

        [Test]
        public void DrawDifferences_DifferencesIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _imagePainter.DrawDifferences(new Bitmap(1, 1), null));
        }

        [Test]
        public void DrawDifferences_DifferencesEmpty_ImageIsNotModified()
        {
            var copy = new Bitmap(_image);
            _imagePainter.DrawDifferences(copy, Enumerable.Empty<RectangleF>());
            
            Assert.AreEqual(_image.Size, copy.Size);
            for(var xIndex = 0; xIndex < _image.Width; xIndex++)
            for (var yIndex = 0; yIndex < _image.Height; yIndex++)
                Assert.AreEqual(_image.GetPixel(xIndex, yIndex).ToArgb(), copy.GetPixel(xIndex, yIndex).ToArgb());
        }

        [Test]
        public void DrawDifferences_DifferencesContainsElement_ImageContainsRectangle()
        {
            var copy = new Bitmap(_image);
            _imagePainter.DrawDifferences(copy, new[] {new RectangleF(0, 0, 1, 1)});
            
            Assert.AreEqual(_image.Size, copy.Size);
            for(var xIndex = 0; xIndex < _image.Width; xIndex++)
            for (var yIndex = 0; yIndex < _image.Height; yIndex++)
                Assert.AreEqual(xIndex < 2 && yIndex < 2 ? _color.ToArgb() : _image.GetPixel(xIndex, yIndex).ToArgb(),
                    copy.GetPixel(xIndex, yIndex).ToArgb());
        }
        
        [Test]
        public void DrawDifferencesAsync_ImageIsNull_ThrowsArgumentNullException()
        {
            var asyncEnumerableMock = new Mock<IAsyncEnumerable<RectangleF>>();
            var asyncEnumeratorMock = new Mock<IAsyncEnumerator<RectangleF>>();
            asyncEnumerableMock.Setup(p => p.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(() => asyncEnumeratorMock.Object);
            asyncEnumeratorMock.Setup(p => p.MoveNextAsync()).ReturnsAsync(() => false);
            
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _imagePainter.DrawDifferencesAsync(null, asyncEnumerableMock.Object, _lockObject));
        }

        [Test]
        public void DrawDifferencesAsync_DifferencesIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _imagePainter.DrawDifferencesAsync(new Bitmap(1, 1), null, _lockObject));
        }

        [Test]
        public async Task DrawDifferencesAsync_DifferencesEmpty_ImageIsNotModified()
        {
            var asyncEnumerableMock = new Mock<IAsyncEnumerable<RectangleF>>();
            var asyncEnumeratorMock = new Mock<IAsyncEnumerator<RectangleF>>();
            asyncEnumerableMock.Setup(p => p.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(() => asyncEnumeratorMock.Object);
            asyncEnumeratorMock.Setup(p => p.MoveNextAsync()).ReturnsAsync(() => false);
            var copy = new Bitmap(_image);
            await _imagePainter.DrawDifferencesAsync(copy, asyncEnumerableMock.Object, _lockObject);
            
            Assert.AreEqual(_image.Size, copy.Size);
            for(var xIndex = 0; xIndex < _image.Width; xIndex++)
            for (var yIndex = 0; yIndex < _image.Height; yIndex++)
                Assert.AreEqual(_image.GetPixel(xIndex, yIndex).ToArgb(), copy.GetPixel(xIndex, yIndex).ToArgb());
        }

        [Test]
        public async Task DrawDifferencesAsync_DifferencesContainsElement_ImageContainsRectangle()
        {
            var asyncEnumerableMock = new Mock<IAsyncEnumerable<RectangleF>>();
            var asyncEnumeratorMock = new Mock<IAsyncEnumerator<RectangleF>>();
            asyncEnumerableMock.Setup(p => p.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(() => asyncEnumeratorMock.Object);
            var returned = false;
            asyncEnumeratorMock.Setup(p => p.MoveNextAsync()).ReturnsAsync(() => !returned).Callback(() => returned = true);
            asyncEnumeratorMock.Setup(p => p.Current).Returns(() => new RectangleF(0, 0, 1, 1));
            var copy = new Bitmap(_image);
            await _imagePainter.DrawDifferencesAsync(copy, asyncEnumerableMock.Object, _lockObject).ConfigureAwait(false);
            
            Assert.AreEqual(_image.Size, copy.Size);
            for(var xIndex = 0; xIndex < _image.Width; xIndex++)
            for (var yIndex = 0; yIndex < _image.Height; yIndex++)
                Assert.AreEqual(xIndex < 2 && yIndex < 2 ? _color.ToArgb() : _image.GetPixel(xIndex, yIndex).ToArgb(),
                    copy.GetPixel(xIndex, yIndex).ToArgb());
        }
    }
}