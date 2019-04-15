using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using ImageComparer;
using Moq;
using NUnit.Framework;

namespace Tests
{
    public class GridImageComparerTests
    {
        private const int Width = 100;

        private const int Height = 100;

        private const int Threshold = 10;

        private const int XSplit = 16;

        private const int YSplit = 16;

        private IImageComparer _comparer;

        private Mock<IImageComparer> _pixelByPixelComparerMock;

        private object _lockObject;

        [SetUp]
        public void SetUp()
        {
            _lockObject = new object();
            _pixelByPixelComparerMock = new Mock<IImageComparer>();
            _comparer = new GridImageComparer(_pixelByPixelComparerMock.Object, Threshold, XSplit, YSplit);
        }

        [Test]
        public void GetDifferences_LeftIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _comparer.GetDifferences(null, new Bitmap(Width, Height)));
        }

        [Test]
        public void GetDifferences_RightIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _comparer.GetDifferences(new Bitmap(Width, Height), null));
        }

        [Test]
        public void GetDifferences_SizeOfLeftAndRightIsNotEqual_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _comparer.GetDifferences(new Bitmap(Width, Height), new Bitmap(Width + 1, Height + 1)));
        }

        [Test]
        public void GetDifferences_SizeOfLeftAndRightIsZero_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _comparer.GetDifferences(new Bitmap(0, 0), new Bitmap(0, 0)));
        }

        [Test]
        public void GetDifferences_CallsPixelByPixelComparerWithHashedImages()
        {
            var leftSize = new SizeF();
            var rightSize = new SizeF();
            _pixelByPixelComparerMock.Setup(p => p.GetDifferences(It.IsAny<Image>(), It.IsAny<Image>()))
                .Callback<Image, Image>((left, right) =>
                {
                    leftSize = left.Size;
                    rightSize = right.Size;
                })
                .Returns(Enumerable.Empty<RectangleF>);

            _comparer.GetDifferences(new Bitmap(Width, Height), new Bitmap(Width, Height));

            _pixelByPixelComparerMock.Verify(p => p.GetDifferences(It.IsAny<Image>(), It.IsAny<Image>()), Times.Once);
            Assert.AreEqual(XSplit, leftSize.Width);
            Assert.AreEqual(XSplit, rightSize.Width);
            Assert.AreEqual(YSplit, leftSize.Height);
            Assert.AreEqual(YSplit, rightSize.Height);
        }

        [Test]
        public void GetDifferences_PixelByPixelComparerReturnsEmpty_ReturnsEmpty()
        {
            _pixelByPixelComparerMock.Setup(p => p.GetDifferences(It.IsAny<Image>(), It.IsAny<Image>()))
                .Returns(() => Enumerable.Empty<RectangleF>());

            var result = _comparer.GetDifferences(new Bitmap(Width, Height), new Bitmap(Width, Height)).ToList();

            Assert.IsEmpty(result);
        }

        [Test]
        public void GetDifferences_PixelByPixelComparerReturnsAllGrid_ReturnsAllGrid()
        {
            _pixelByPixelComparerMock.Setup(p => p.GetDifferences(It.IsAny<Image>(), It.IsAny<Image>())).Returns(() =>
            {
                var res = new Collection<RectangleF>();
                for (var xIndex = 0; xIndex < XSplit; xIndex++)
                for (var yIndex = 0; yIndex < YSplit; yIndex++)
                    res.Add(new Rectangle(xIndex, yIndex, 1, 1));

                return res;
            });

            var result = _comparer.GetDifferences(new Bitmap(Width, Height), new Bitmap(Width, Height)).ToList();

            Assert.AreEqual(XSplit * YSplit, result.Count);
            for (var xIndex = 0; xIndex < XSplit; xIndex++)
            for (var yIndex = 0; yIndex < YSplit; yIndex++)
                Assert.IsTrue(result.Contains(new RectangleF(xIndex * (float) Width / XSplit,
                    yIndex * (float) Height / YSplit, (float) Width / XSplit, (float) Height / YSplit)));
        }

        [Test]
        public void GetDifferences_PixelByPixelComparerReturnsPartOfGrid_ReturnsPartOfGrid()
        {
            var calls = 0;
            const int callsLimit = (XSplit + YSplit) / 2;
            _pixelByPixelComparerMock.Setup(p => p.GetDifferences(It.IsAny<Image>(), It.IsAny<Image>())).Returns(() =>
            {
                var res = new Collection<RectangleF>();
                for (var xIndex = 0; xIndex < XSplit; xIndex++)
                for (var yIndex = 0; yIndex < YSplit; yIndex++)
                    if (++calls <= callsLimit)
                        res.Add(new Rectangle(xIndex, yIndex, 1, 1));

                return res;
            });

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
            Assert.ThrowsAsync<ArgumentException>(async () => await
                _comparer.GetDifferencesAsync(new Bitmap(Width, Height), new Bitmap(Width + 1, Height + 1), _lockObject)
                    .IterateToListAsync());
        }

        [Test]
        public void GetDifferencesAsync_SizeOfLeftAndRightIsZero_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _comparer.GetDifferencesAsync(new Bitmap(0, 0), new Bitmap(0, 0), _lockObject).IterateToListAsync());
        }

        [Test]
        public void GetDifferencesAsync_CallsPixelByPixelComparerWithHashedImages()
        {
            var leftSize = new SizeF();
            var rightSize = new SizeF();
            var asyncEnumerableMock = new Mock<IAsyncEnumerable<RectangleF>>();
            var asyncEnumeratorMock = new Mock<IAsyncEnumerator<RectangleF>>();
            asyncEnumerableMock.Setup(p => p.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(() => asyncEnumeratorMock.Object);
            asyncEnumeratorMock.Setup(p => p.MoveNextAsync()).ReturnsAsync(() => false);
            _pixelByPixelComparerMock.Setup(p => p.GetDifferencesAsync(It.IsAny<Image>(), It.IsAny<Image>(), _lockObject))
                .Callback<Image, Image, object>((left, right, lockObject) =>
                {
                    leftSize = left.Size;
                    rightSize = right.Size;
                })
                .Returns(() => asyncEnumerableMock.Object);

            var _ = _comparer.GetDifferencesAsync(new Bitmap(Width, Height), new Bitmap(Width, Height), _lockObject)
                .IterateToListAsync().Result;

            _pixelByPixelComparerMock.Verify(p => p.GetDifferencesAsync(It.IsAny<Image>(), It.IsAny<Image>(), _lockObject),
                Times.Once);
            Assert.AreEqual(XSplit, leftSize.Width);
            Assert.AreEqual(XSplit, rightSize.Width);
            Assert.AreEqual(YSplit, leftSize.Height);
            Assert.AreEqual(YSplit, rightSize.Height);
        }

        [Test]
        public void GetDifferencesAsync_PixelByPixelComparerReturnsEmpty_ReturnsEmpty()
        {
            var asyncEnumerableMock = new Mock<IAsyncEnumerable<RectangleF>>();
            var asyncEnumeratorMock = new Mock<IAsyncEnumerator<RectangleF>>();
            asyncEnumerableMock.Setup(p => p.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(() => asyncEnumeratorMock.Object);
            asyncEnumeratorMock.Setup(p => p.MoveNextAsync()).ReturnsAsync(() => false);
            _pixelByPixelComparerMock.Setup(p => p.GetDifferencesAsync(It.IsAny<Image>(), It.IsAny<Image>(), _lockObject))
                .Returns(() => asyncEnumerableMock.Object);

            var result = _comparer.GetDifferencesAsync(new Bitmap(Width, Height), new Bitmap(Width, Height), _lockObject)
                .IterateToListAsync().Result;

            Assert.IsEmpty(result);
        }

        [Test]
        public void GetDifferencesAsync_PixelByPixelComparerReturnsAllGrid_ReturnsAllGrid()
        {
            var mockResult = new Collection<RectangleF>();
            for (var xIndex = 0; xIndex < XSplit; xIndex++)
            for (var yIndex = 0; yIndex < YSplit; yIndex++)
                mockResult.Add(new Rectangle(xIndex, yIndex, 1, 1));
            var mockResultEnumerator = mockResult.GetEnumerator();

            var asyncEnumerableMock = new Mock<IAsyncEnumerable<RectangleF>>();
            var asyncEnumeratorMock = new Mock<IAsyncEnumerator<RectangleF>>();
            asyncEnumerableMock.Setup(p => p.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(() => asyncEnumeratorMock.Object);
            asyncEnumeratorMock.Setup(p => p.MoveNextAsync()).ReturnsAsync(() => mockResultEnumerator.MoveNext());
            asyncEnumeratorMock.Setup(p => p.Current).Returns(() => mockResultEnumerator.Current);
            _pixelByPixelComparerMock.Setup(p => p.GetDifferencesAsync(It.IsAny<Image>(), It.IsAny<Image>(), _lockObject))
                .Returns(() => asyncEnumerableMock.Object);

            var result = _comparer.GetDifferencesAsync(new Bitmap(Width, Height), new Bitmap(Width, Height), _lockObject)
                .IterateToListAsync().Result;

            Assert.AreEqual(XSplit * YSplit, result.Count);
            for (var xIndex = 0; xIndex < XSplit; xIndex++)
            for (var yIndex = 0; yIndex < YSplit; yIndex++)
                Assert.IsTrue(result.Contains(new RectangleF(xIndex * (float) Width / XSplit,
                    yIndex * (float) Height / YSplit, (float) Width / XSplit, (float) Height / YSplit)));
        }

        [Test]
        public void GetDifferencesAsync_PixelByPixelComparerReturnsPartOfGrid_ReturnsPartOfGrid()
        {
            var calls = 0;
            const int callsLimit = (XSplit + YSplit) / 2;
            var mockResult = new Collection<RectangleF>();
            for (var xIndex = 0; xIndex < XSplit; xIndex++)
            for (var yIndex = 0; yIndex < YSplit; yIndex++)
                if (++calls <= callsLimit)
                    mockResult.Add(new Rectangle(xIndex, yIndex, 1, 1));
            var mockResultEnumerator = mockResult.GetEnumerator();

            var asyncEnumerableMock = new Mock<IAsyncEnumerable<RectangleF>>();
            var asyncEnumeratorMock = new Mock<IAsyncEnumerator<RectangleF>>();
            asyncEnumerableMock.Setup(p => p.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(() => asyncEnumeratorMock.Object);
            asyncEnumeratorMock.Setup(p => p.MoveNextAsync()).ReturnsAsync(() => mockResultEnumerator.MoveNext());
            asyncEnumeratorMock.Setup(p => p.Current).Returns(() => mockResultEnumerator.Current);
            _pixelByPixelComparerMock.Setup(p => p.GetDifferencesAsync(It.IsAny<Image>(), It.IsAny<Image>(), _lockObject))
                .Returns(() => asyncEnumerableMock.Object);

            var result = _comparer.GetDifferencesAsync(new Bitmap(Width, Height), new Bitmap(Width, Height), _lockObject)
                .IterateToListAsync().Result;

            Assert.AreEqual(callsLimit, result.Count);
        }
    }
}