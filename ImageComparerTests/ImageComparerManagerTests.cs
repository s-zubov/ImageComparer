using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using ImageComparer;
using ImageComparer.Algorithms;
using ImageComparer.Storage;
using Moq;
using NUnit.Framework;

namespace ImageComparerTests
{
    public class ImageComparerManagerTests
    {
        private IImageComparerManager _comparerManager;
        
        private Mock<IImageComparerAlgorithm> _comparerAlgorithmMock;
        
        private Mock<IImagePainter> _painterMock;
        
        private Mock<IImageStorage> _storageMock;
        
        private Bitmap _image;
        
        private const int Threshold = 20;

        [SetUp]
        public void SetUp()
        {
            _comparerAlgorithmMock = new Mock<IImageComparerAlgorithm>();
            _painterMock = new Mock<IImagePainter>();
            _storageMock = new Mock<IImageStorage>();
            _comparerManager = new ImageComparerManager(_comparerAlgorithmMock.Object, _painterMock.Object, _storageMock.Object);
            _image = new Bitmap(100, 100);
        }

        [Test]
        public void Process_LeftIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _comparerManager.Process(null, _image, Threshold));
        }
        
        [Test]
        public void Process_RightIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _comparerManager.Process(_image, null, Threshold));
        }

        
        [Test]
        public void Process_CallsToComparerAlgorithm()
        {
            _comparerManager.Process(_image, _image, Threshold);
            
            _comparerAlgorithmMock.Verify(p => p.GetDifferences(_image, _image, Threshold), Times.Once);
        }
        
        [Test]
        public void Process_ComparerAlgorithmReturns_CallsToPainter()
        {
            var rects = Enumerable.Repeat(new RectangleF(1, 2, 3, 4), 5);
            _comparerAlgorithmMock.Setup(p => p.GetDifferences(_image, _image, Threshold)).Returns(rects);
            
            _comparerManager.Process(_image, _image, Threshold);
            
            _painterMock.Verify(p => p.DrawDifferences(_image, rects), Times.Once);
        }
        
        [Test]
        public void Process_CallsToStorage()
        {
            _comparerManager.Process(_image, _image, Threshold);
            
            _storageMock.Verify(p => p.Create(It.IsAny<BitmapAndLock>()), Times.Once);
        }
        
        [Test]
        public void ProcessInBackground_LeftIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _comparerManager.ProcessInBackground(null, _image, Threshold));
        }
        
        [Test]
        public void ProcessInBackground_RightIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _comparerManager.ProcessInBackground(_image, null, Threshold));
        }
        
        [Test]
        public void ProcessInBackground_CallsToComparerAlgorithm()
        {
            _comparerManager.ProcessInBackground(_image, _image, Threshold);

            _comparerManager.ImageProcessed += (sender, args) =>
                _comparerAlgorithmMock.Verify(p => p.GetDifferencesAsync(_image, _image, Threshold, It.IsAny<object>()),
                    Times.Once);
        }
        
        [Test]
        public void ProcessInBackground_ComparerAlgorithmReturns_CallsToPainter()
        {
            var mockResult = Enumerable.Repeat(new RectangleF(1, 2, 3, 4), 5).ToList();
            var mockResultEnumerator = mockResult.GetEnumerator();
            var asyncEnumerableMock = new Mock<IAsyncEnumerable<RectangleF>>();
            var asyncEnumeratorMock = new Mock<IAsyncEnumerator<RectangleF>>();
            asyncEnumerableMock.Setup(p => p.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(() => asyncEnumeratorMock.Object);
            asyncEnumeratorMock.Setup(p => p.MoveNextAsync()).ReturnsAsync(() => mockResultEnumerator.MoveNext());
            asyncEnumeratorMock.Setup(p => p.Current).Returns(() => mockResultEnumerator.Current);
            
            _comparerAlgorithmMock.Setup(p => p.GetDifferencesAsync(_image, _image, Threshold, It.IsAny<object>())).Returns(asyncEnumerableMock.Object);
            
            _comparerManager.ProcessInBackground(_image, _image, Threshold);

            _comparerManager.ImageProcessed += (sender, args) =>
                _painterMock.Verify(
                    p => p.DrawDifferencesAsync(_image, asyncEnumerableMock.Object, It.IsAny<object>()),
                    Times.Once);
        }
        
        [Test]
        public void ProcessInBackground_CallsToStorage()
        {
            _comparerManager.ProcessInBackground(_image, _image, Threshold);
            
            _storageMock.Verify(p => p.Create(It.IsAny<BitmapAndLock>()), Times.Once);
        }
    }
}