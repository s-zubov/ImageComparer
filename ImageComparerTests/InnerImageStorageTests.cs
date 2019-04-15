using System;
using System.Drawing;
using ImageComparer;
using NUnit.Framework;

namespace Tests
{
    public class InnerImageStorageTests
    {
        private IImageStorage _imageStorage;
        
        private Bitmap _image;

        private object _lockObject;

        [SetUp]
        public void SetUp()
        {
            _imageStorage = new ImageStorage();
            _image = new Bitmap(100, 100);
            _lockObject = new object();
        }

        [Test]
        public void Save_ImageIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _imageStorage.Create(null));
        }

        [Test]
        public void SaveAndLoad_ImageIsNotNull_ImageReturned()
        {
            var result = _imageStorage.Read(_imageStorage.Create(new BitmapAndLock(_image, _lockObject)));
            
            Assert.NotNull(result);
            Assert.AreEqual(_image.Size, result.Bitmap.Size);
            for(var xIndex = 0; xIndex < _image.Width; xIndex++)
            for (var yIndex = 0; yIndex < _image.Height; yIndex++)
                Assert.AreEqual(_image.GetPixel(xIndex, yIndex).ToArgb(), result.Bitmap.GetPixel(xIndex, yIndex).ToArgb());
        }

        [Test]
        public void Load_NewImage_ReturnsNull()
        {
            Assert.IsNull(_imageStorage.Read(Guid.NewGuid()));
        }

        
        [Test]
        public void Delete_ImageNotSaved_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _imageStorage.Delete(Guid.NewGuid()));
        }

        [Test]
        public void DeleteAndLoad_ImageSaved_ReturnsNull()
        {
            var guid = _imageStorage.Create(new BitmapAndLock(_image, _lockObject));
            _imageStorage.Delete(guid);
            
            Assert.IsNull(_imageStorage.Read(guid));
        }
    }
}