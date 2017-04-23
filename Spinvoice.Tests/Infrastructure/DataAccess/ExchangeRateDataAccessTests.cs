using System;
using System.Linq;
using NUnit.Framework;
using Spinvoice.Domain.Exchange;
using Spinvoice.Infrastructure.DataAccess;

namespace Spinvoice.Tests.Infrastructure.DataAccess
{
    [TestFixture]
    public class ExchangeRateDataAccessTests
    {
        private ExchangeRateDataAccess _dataAccess;

        [SetUp]
        public void Setup()
        {
            var documentStoreRepository = new DocumentStoreRepository("Data");
            _dataAccess = new ExchangeRateDataAccess(documentStoreRepository);
            _dataAccess.DeleteAll();
        }

        [TearDown]
        public void TearDown()
        {
            _dataAccess.DeleteAll();
            _dataAccess?.Dispose();
        }

        [Test]
        public void GetsEmptyCompanyArrayIfNothingAdded()
        {
            var rate = new Rate
            {
                Currency = "USD",
                Date = new DateTime(2017, 4, 12),
                Value = 1.02m
            };
            _dataAccess.AddOrUpdate(rate);

            var loadedRate = _dataAccess.GetRate("USD", new DateTime(2017, 4, 12));

            Assert.IsNotNull(loadedRate);
            Assert.AreEqual(1.02m, loadedRate.Value);
        }

        [Test]
        public void DeletesAllRates()
        {
            var rates = Enumerable.Range(1, 200).Select(i => new Rate
            {
                Date = new DateTime(2016, 1, 1).AddDays(1)
            }).ToArray();
            _dataAccess.AddOrUpdate(rates);
            _dataAccess.GetAll();

            _dataAccess.DeleteAll();

            var loadedRates = _dataAccess.GetAll();
            Assert.AreEqual(0, loadedRates.Length);
        }

    }
}