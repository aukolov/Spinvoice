using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Spinvoice.Domain.Exchange;
// ReSharper disable PossibleNullReferenceException

namespace Spinvoice.App.Services
{
    public class ExchangeRatesLoader
    {
        private readonly IExchangeRateDataAccess _exchangeRateDataAccess;
        private readonly XNamespace _ns = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref";
        private readonly XNamespace _gesmes = "http://www.gesmes.org/xml/2002-08-01";

        public ExchangeRatesLoader(IExchangeRateDataAccess exchangeRateDataAccess)
        {
            _exchangeRateDataAccess = exchangeRateDataAccess;
        }

        public void Load(string filePath)
        {
            _exchangeRateDataAccess.DeleteAll();

            var xml = File.ReadAllText(filePath);
            var document = XDocument.Parse(xml);
            var envelope = document.Element(_gesmes + "Envelope");

            var rates = new List<Rate>();

            int i = 0;
            foreach (var dateCube in envelope.Element(_ns + "Cube").Elements(_ns + "Cube"))
            {
                var stringDate = dateCube.Attribute("time").Value;
                var date = DateTime.ParseExact(stringDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                foreach (var currencyCube in dateCube.Elements(_ns + "Cube"))
                {
                    var currency = currencyCube.Attribute("currency").Value;
                    var rate = decimal.Parse(currencyCube.Attribute("rate").Value);

                    rates.Add(new Rate
                    {
                        Currency = currency,
                        Date = date,
                        Value = rate
                    });
                }
                i++;
            }

            _exchangeRateDataAccess.AddOrUpdate(rates);

        }
    }
}
