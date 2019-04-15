using System.Drawing;
using ImageComparer;
using NUnit.Framework;

namespace Tests
{
    public class ArgbPixelComparerTests
    {
        private IPixelComparerAlgorithm _argbPixelComparerAlgorithm;

        [SetUp]
        public void Setup()
        {
            _argbPixelComparerAlgorithm = new ArgbPixelComparerAlgorithm();
        }

        [Test]
        public void PixelEquals_SameColorNoThreshold_ReturnsTrue()
        {
            Assert.IsTrue(_argbPixelComparerAlgorithm.PixelEquals(Color.Red, Color.Red));
        }

        [Test]
        public void PixelEquals_DifferentColorNoThreshold_ReturnsFalse()
        {
            Assert.IsFalse(_argbPixelComparerAlgorithm.PixelEquals(Color.Red, Color.Tan));
        }

        [Test]
        public void PixelEquals_SameRgbDifferentAlphaNoThreshold_ReturnsFalse()
        {
            var left = Color.FromArgb(10, 100, 100, 100);
            var right = Color.FromArgb(50, 100, 100, 100);

            Assert.IsFalse(_argbPixelComparerAlgorithm.PixelEquals(left, right));
        }

        [Test]
        public void PixelEquals_SameColorZeroThreshold_ReturnsTrue()
        {
            Assert.IsTrue(_argbPixelComparerAlgorithm.PixelEquals(Color.Red, Color.Red, 0));
        }

        [Test]
        public void PixelEquals_DifferentColorZeroThreshold_ReturnsFalse()
        {
            Assert.IsFalse(_argbPixelComparerAlgorithm.PixelEquals(Color.Red, Color.Tan, 0));
        }

        [Test]
        public void PixelEquals_SameColorWithThreshold_ReturnsTrue()
        {
            Assert.IsTrue(_argbPixelComparerAlgorithm.PixelEquals(Color.Red, Color.Red, 1));
        }

        [Test]
        public void PixelEquals_DifferentByAlphaLesserThanThresholdColors_ReturnsTrue()
        {
            var left = Color.FromArgb(1, 100, 100, 100);
            var right = Color.FromArgb(2, 100, 100, 100);

            Assert.IsTrue(_argbPixelComparerAlgorithm.PixelEquals(left, right, 2));
        }

        [Test]
        public void PixelEquals_DifferentByRgbLesserThanThresholdColors_ReturnsTrue()
        {
            // sqrt(2^2 + 2^2 + 2^2) ~ 3.46 < 4
            var left = Color.FromArgb(1, 100, 100, 100);
            var right = Color.FromArgb(1, 98, 98, 98);

            Assert.IsTrue(_argbPixelComparerAlgorithm.PixelEquals(left, right, 4));
        }

        [Test]
        public void PixelEquals_DifferentByArgbLesserThanThresholdColors_ReturnsTrue()
        {
            var left = Color.FromArgb(1, 100, 100, 100);
            var right = Color.FromArgb(2, 98, 98, 98);

            Assert.IsTrue(_argbPixelComparerAlgorithm.PixelEquals(left, right, 5));
        }

        [Test]
        public void PixelEquals_DifferentByAlphaEqualToThresholdColors_ReturnsTrue()
        {
            var left = Color.FromArgb(1, 100, 100, 100);
            var right = Color.FromArgb(2, 100, 100, 100);

            Assert.IsTrue(_argbPixelComparerAlgorithm.PixelEquals(left, right, 1));
        }

        [Test]
        public void PixelEquals_DifferentByRgbEqualToThresholdColors_ReturnsTrue()
        {
            var left = Color.FromArgb(1, 100, 100, 100);
            var right = Color.FromArgb(1, 98, 100, 100);

            Assert.IsTrue(_argbPixelComparerAlgorithm.PixelEquals(left, right, 2));
        }

        [Test]
        public void PixelEquals_DifferentByArgbEqualToThresholdColors_ReturnsTrue()
        {
            var left = Color.FromArgb(1, 100, 100, 100);
            var right = Color.FromArgb(2, 100, 98, 100);

            Assert.IsTrue(_argbPixelComparerAlgorithm.PixelEquals(left, right, 3));
        }

        [Test]
        public void PixelEquals_DifferentByAlphaBiggerThanThresholdColors_ReturnsFalse()
        {
            var left = Color.FromArgb(1, 100, 100, 100);
            var right = Color.FromArgb(3, 100, 100, 100);

            Assert.IsFalse(_argbPixelComparerAlgorithm.PixelEquals(left, right, 1));
        }

        [Test]
        public void PixelEquals_DifferentByRgbBiggerThanThresholdColors_ReturnsFalse()
        {
            // sqrt(2^2 + 2^2 + 2^2) ~ 3.46 < 4
            var left = Color.FromArgb(1, 100, 100, 100);
            var right = Color.FromArgb(1, 98, 98, 98);

            Assert.IsFalse(_argbPixelComparerAlgorithm.PixelEquals(left, right, 3));
        }

        [Test]
        public void PixelEquals_DifferentByArgbBiggerThanThresholdColors_ReturnsFalse()
        {
            var left = Color.FromArgb(1, 100, 100, 100);
            var right = Color.FromArgb(2, 98, 98, 98);

            Assert.IsFalse(_argbPixelComparerAlgorithm.PixelEquals(left, right, 4));
        }
    }
}