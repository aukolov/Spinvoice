using System;
using Intuit.Ipp.Data;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.Invoice
{
    public class ExternalInvoiceAndBillService : IExternalInvoiceAndBillService
    {
        private readonly IExternalInvoiceUpdater _externalInvoiceUpdater;
        private readonly IExternalConnection _externalConnection;

        public ExternalInvoiceAndBillService(
            IExternalInvoiceUpdater externalInvoiceUpdater,
            IExternalConnection externalConnection)
        {
            _externalInvoiceUpdater = externalInvoiceUpdater;
            _externalConnection = externalConnection;
        }

        public string Save(Spinvoice.Domain.Accounting.Invoice invoice)
        {
            switch (invoice.Side)
            {
                case Side.Vendor:
                    return invoice.ExternalId == null ? AddIBill(invoice) : UpdateBill(invoice);
                case Side.Customer:
                    return invoice.ExternalId == null ? AddInvoice(invoice) : UpdateInvoice(invoice);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string AddIBill(Spinvoice.Domain.Accounting.Invoice invoice)
        {
            var bill = new Bill();
            _externalInvoiceUpdater.Update(invoice, bill);
            return _externalConnection.Add(bill).Id;
        }

        private string UpdateBill(Spinvoice.Domain.Accounting.Invoice invoice)
        {
            var bill = _externalConnection.GetBill(invoice.ExternalId);
            _externalInvoiceUpdater.Update(invoice, bill);
            _externalConnection.Update(bill);
            return invoice.ExternalId;
        }

        private string AddInvoice(Spinvoice.Domain.Accounting.Invoice invoice)
        {
            var externalInvoice = new Intuit.Ipp.Data.Invoice();
            _externalInvoiceUpdater.Update(invoice, externalInvoice);
            return _externalConnection.Add(externalInvoice).Id;
        }

        private string UpdateInvoice(Spinvoice.Domain.Accounting.Invoice invoice)
        {
            var externalInvoice = _externalConnection.GetInvoice(invoice.ExternalId);
            _externalInvoiceUpdater.Update(invoice, externalInvoice);
            _externalConnection.Update(externalInvoice);
            return invoice.ExternalId;
        }
    }
}