using NUnit.Framework;
using NUnit.Framework.Internal;
using Spinvoice.Utils;

namespace Spinvoice.Tests.Domain.Utils
{
    [TestFixture()]
    public class AmountParserTests
    {
        [Test]
        public void ParsesThousandsDelimiter()
        {
            var result = AmountParser.Parse("3.799,98");
            Assert.AreEqual(3799.98m, result);
        }

        [Test]
        public void ParsesDotAsDeciamlDelimiter()
        {
            var result = AmountParser.Parse("1.98");
            Assert.AreEqual(1.98m, result);
        }

        [Test]
        public void ParsesCommaAsDeciamlDelimiter()
        {
            var result = AmountParser.Parse("1,98");
            Assert.AreEqual(1.98m, result);
        }

        [Test]
        public void ParsesSpaceAsDeciamlDelimiter()
        {
            var result = AmountParser.Parse("1 123,45");
            Assert.AreEqual(1123.45m, result);
        }
        
        [Test]
        public void ParsesAmountWithUsdCurrencyPrefix()
        {
            var result = AmountParser.Parse("USD 1000");
            Assert.AreEqual(1000m, result);
        }
    }
}