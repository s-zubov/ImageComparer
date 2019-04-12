using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ImageComparer;
using NUnit.Framework;

namespace Tests
{
    public class PointComparerTests
    {
        private PointComparer _pointComparer;

        private const int Positive = 42;

        private const int Zero = 0;

        private const int Negative = -42;

        private const int SmallerPositive = Positive - 1;

        private const int SmallerNegative = Negative - 1;

        private readonly List<int> _testNumbers =
            new[] {Positive, SmallerPositive, Zero, Negative, SmallerNegative}.ToList();

        [SetUp]
        public void SetUp()
        {
            _pointComparer = new PointComparer();
        }
        
        [Test]
        public void CompareTo_X1GtX2AndY1GtY2_ReturnsGtz()
        {
            _testNumbers.ForEach(greater => _testNumbers.Where(lesser => greater > lesser).ToList().ForEach(lesser =>
                Assert.IsTrue(_pointComparer.Compare(new Point(greater, greater), new Point(lesser, lesser)) > 0)));
        }
        
        [Test]
        public void CompareTo_X1GtX2AndY1EqualsY2_ReturnsGtz()
        {
            _testNumbers.ForEach(greater => _testNumbers.Where(lesser => greater > lesser).ToList().ForEach(lesser =>
                Assert.IsTrue(_pointComparer.Compare(new Point(greater, greater), new Point(lesser, greater)) > 0)));
        }
        
        [Test]
        public void CompareTo_X1GtX2AndY1LtY2_ReturnsGtz()
        {
            _testNumbers.ForEach(greater => _testNumbers.Where(lesser => greater > lesser).ToList().ForEach(lesser =>
                Assert.IsTrue(_pointComparer.Compare(new Point(greater, lesser), new Point(lesser, greater)) > 0)));
        }
        
        [Test]
        public void CompareTo_X1EqualsX2AndY1GtY2_ReturnsGtz()
        {
            _testNumbers.ForEach(greater => _testNumbers.Where(lesser => greater > lesser).ToList().ForEach(lesser =>
                Assert.IsTrue(_pointComparer.Compare(new Point(greater, greater), new Point(greater, lesser)) > 0)));
        }
        
        [Test]
        public void CompareTo_X1EqualsX2AndX1Y1EqualsY2_ReturnsZero()
        {
            _testNumbers.ForEach(greater => Assert.IsTrue(_pointComparer.Compare(new Point(greater, greater), new Point(greater, greater)) == 0));
        }
        
        [Test]
        public void CompareTo_X1EqualsX2AndX1Y1LtY2_ReturnsLtz()
        {
            _testNumbers.ForEach(greater => _testNumbers.Where(lesser => greater > lesser).ToList().ForEach(lesser =>
                Assert.IsTrue(_pointComparer.Compare(new Point(greater, lesser), new Point(greater, greater)) < 0)));
        }
        
        [Test]
        public void CompareTo_X1LtX2AndX1Y1GtY2_ReturnsLtz()
        {
            _testNumbers.ForEach(greater => _testNumbers.Where(lesser => greater > lesser).ToList().ForEach(lesser =>
                Assert.IsTrue(_pointComparer.Compare(new Point(lesser, lesser), new Point(greater, greater)) < 0)));
        }
        
        [Test]
        public void CompareTo_X1LtX2AndX1Y1EqualsY2_ReturnsLtz()
        {
            _testNumbers.ForEach(greater => _testNumbers.Where(lesser => greater > lesser).ToList().ForEach(lesser =>
                Assert.IsTrue(_pointComparer.Compare(new Point(lesser, lesser), new Point(greater, lesser)) < 0)));
        }
        
        [Test]
        public void CompareTo_X1LtX2AndX1Y1LtY2_ReturnsLtz()
        {
            _testNumbers.ForEach(greater => _testNumbers.Where(lesser => greater > lesser).ToList().ForEach(lesser =>
                Assert.IsTrue(_pointComparer.Compare(new Point(lesser, greater), new Point(greater, lesser)) < 0)));
        }
    }
}