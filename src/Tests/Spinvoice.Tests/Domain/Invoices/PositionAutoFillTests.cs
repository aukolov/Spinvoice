using FluentAssertions;
using NUnit.Framework;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.Invoices;
using Spinvoice.QuickBooks.Reporting;

namespace Spinvoice.Tests.Domain.Invoices
{
    [TestFixture]
    public class PositionAutoFillTests
    {
        private PositionAutoFill _service;

        [SetUp]
        public void SetUp()
        {
            _service = new PositionAutoFill();
        }

        private static InventoryValuationItem Createitem(string name, int quantity, int amount)
        {
            return new InventoryValuationItem("1", name, quantity, amount);
        }

        private static void AssertPosition(Position position, int quantity, decimal amount)
        {
            position.Quantity.Should().Be(quantity);
            position.Amount.Should().Be(amount);
        }

        private Position[] FillPositions(int totalAmount, params InventoryValuationItem[] items)
        {
            return _service.FillPositions(
                items,
                totalAmount, 1, 0);
        }

        [Test]
        public void FillsFromOneItem()
        {
            var positions = FillPositions(1200, Createitem("test1", 10, 1000));

            positions.Should().HaveCount(1);
            AssertPosition(positions[0], 10, 1000);
        }

        [Test]
        public void FillsFromTwoItems()
        {
            var positions = FillPositions(2500, 
                Createitem("test1", 10, 1000),
                Createitem("test2", 20, 1200));

            positions.Should().HaveCount(2);
            AssertPosition(positions[0], 10, 1000);
            AssertPosition(positions[1], 20, 1200);
        }

        [Test]
        public void FillsPartiallyFromSecondItem()
        {
            var positions = FillPositions(1600,
                Createitem("test1", 10, 1000),
                Createitem("test2", 20, 1200));

            positions.Should().HaveCount(2);
            AssertPosition(positions[0], 10, 1000);
            AssertPosition(positions[1], 10, 600);
        }

        [Test]
        public void FillsPartiallyFromSecondItemAndAddsDelta()
        {
            var positions = FillPositions(1670,
                Createitem("test1", 10, 1000),
                Createitem("test2", 20, 1200));

            positions.Should().HaveCount(2);
            AssertPosition(positions[0], 10, 1000);
            AssertPosition(positions[1], 11, 670);
        }

        [Test]
        public void FillsPartiallyViseVersa()
        {
            var positions = FillPositions(1670,
                Createitem("test2", 20, 1200),
                Createitem("test1", 10, 1000));

            positions.Should().HaveCount(2);
            AssertPosition(positions[0], 11, 670);
            AssertPosition(positions[1], 10, 1000);
        }

        [Test]
        public void FillsPartiallyFromFirstItemAndAddsDelta()
        {
            var positions = FillPositions(1630,
                Createitem("test1", 10, 1000),
                Createitem("test2", 1, 1200));

            positions.Should().HaveCount(2);
            AssertPosition(positions[0], 4, 430);
            AssertPosition(positions[1], 1, 1200);
        }

        [Test]
        public void StopsIfExactMatchFound()
        {
            var positions = FillPositions(2200,
                Createitem("test1", 10, 1000),
                Createitem("test2", 20, 1200),
                Createitem("test3", 1, 10));

            positions.Should().HaveCount(2);
            AssertPosition(positions[0], 10, 1000);
            AssertPosition(positions[1], 20, 1200);
        }
    }
}