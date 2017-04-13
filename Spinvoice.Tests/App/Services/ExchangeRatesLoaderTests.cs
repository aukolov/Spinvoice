using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Spinvoice.App.Services;
using Spinvoice.Infrastructure.DataAccess;

namespace Spinvoice.Tests.App.Services
{
    [TestFixture]
    public class ExchangeRatesLoaderTests
    {
        private string _tempFileName;
        private ExchangeRatesLoader _exchangeRatesLoader;
        private ExchangeRateDataAccess _exchangeRateDataAccess;

        [SetUp]
        public void Setup()
        {
            _tempFileName = Path.GetTempFileName();
            var documentStoreRepository = new DocumentStoreRepository();
            _exchangeRateDataAccess = new ExchangeRateDataAccess(documentStoreRepository);
            _exchangeRatesLoader = new ExchangeRatesLoader(_exchangeRateDataAccess);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_tempFileName))
            {
                File.Delete(_tempFileName);
            }

            _exchangeRateDataAccess.DeleteAll();
        }

        [Test]
        public void LoadsRates()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetType(), "eurofxref-hist-90d.xml");
            Assert.IsNotNull(stream);
            using (var reader = new StreamReader(stream))
            {
                var xml = reader.ReadToEnd();
                File.WriteAllText(_tempFileName, xml);
            }

            _exchangeRatesLoader.Load(_tempFileName);

            Assert.AreEqual(1.0726m, _exchangeRateDataAccess.GetRate("USD", new DateTime(2017, 03, 16)).Value);
            Assert.AreEqual(121.55, _exchangeRateDataAccess.GetRate("JPY", new DateTime(2017, 03, 16)).Value);
            Assert.AreEqual(0.87808, _exchangeRateDataAccess.GetRate("GBP", new DateTime(2017, 01, 16)).Value);
        }
    }
}