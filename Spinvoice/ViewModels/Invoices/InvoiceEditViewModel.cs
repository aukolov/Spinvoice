using System;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.Company;
using Spinvoice.Domain.Exchange;
using Spinvoice.Utils;

namespace Spinvoice.ViewModels.Invoices
{
    public class InvoiceEditViewModel
    {
        private readonly RawInvoice _rawInvoice;
        private readonly ICompanyRepository _companyRepository;
        private readonly IExchangeRatesRepository _exchangeRatesRepository;

        public Invoice Invoice { get; }
        public ActionSelectorViewModel ActionSelectorViewModel { get; }

        public InvoiceEditViewModel(
            Invoice invoice,
            ActionSelectorViewModel actionSelectorViewModel,
            RawInvoice rawInvoice,
            ICompanyRepository companyRepository,
            IExchangeRatesRepository exchangeRatesRepository)
        {
            _rawInvoice = rawInvoice;
            _companyRepository = companyRepository;
            _exchangeRatesRepository = exchangeRatesRepository;
            Invoice = invoice;
            ActionSelectorViewModel = actionSelectorViewModel;

            Invoice.CurrencyChanged += UpdateRate;
            Invoice.DateChanged += UpdateRate;
        }

        public void ChangeCompanyName(string text)
        {
            Invoice.CompanyName = text;
            _rawInvoice.CompanyName = text;

            var company = _companyRepository.GetByName(Invoice.CompanyName);
            if (company != null)
            {
                Invoice.ApplyCompany(company);
                ActionSelectorViewModel.Advance();
                ActionSelectorViewModel.Advance();
            }
        }

        public void ChangeDate(string text)
        {
            var parsedDate = DateParser.TryParseDate(text);
            if (parsedDate.HasValue)
            {
                Invoice.Date = parsedDate.Value;
                _rawInvoice.Date = text;
                UpdateRate();
            }
        }

        public void ChangeInvoiceNumber(string text)
        {
            Invoice.InvoiceNumber = text;
            _rawInvoice.InvoiceNumber = text;
        }

        public void ChangeCurrency(string text)
        {
            Invoice.Currency = text;
            UpdateRate();
        }

        public void ChangeCountry(string text)
        {
            Invoice.Country = text;
        }

        public void ChangeVatNumber(string text)
        {
            Invoice.VatNumber = text;
        }

        public void ChangeNetAmount(string text)
        {
            decimal netAmount;
            if (!AmountParser.TryParse(text, out netAmount)) return;

            Invoice.NetAmount = netAmount;
            _rawInvoice.NetAmount = text;
        }

        public void ChangeVatAmount(string text)
        {
            Invoice.VatAmount = AmountParser.Parse(text);
        }

        public void ChangeTransportationCosts(string text)
        {
            Invoice.TransportationCosts = AmountParser.Parse(text);
        }

        private void UpdateRate()
        {
            if (Invoice.Date != default(DateTime) && !string.IsNullOrEmpty(Invoice.Currency))
                for (var i = 0; i < 5; i++)
                {
                    var rate = _exchangeRatesRepository.GetRate(Invoice.Currency, Invoice.Date.AddDays(-i));
                    if (!rate.HasValue) continue;

                    Invoice.ExchangeRate = rate.Value;
                    break;
                }
        }
    }
}