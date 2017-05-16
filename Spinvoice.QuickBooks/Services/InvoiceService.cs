using System.Linq;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using Spinvoice.Domain.QuickBooks;
using Invoice = Spinvoice.Domain.Accounting.Invoice;

namespace Spinvoice.QuickBooks.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly OAuthProfile _oauthProfile;
        private readonly InvoiceToBillTranslator _invoiceToBillTranslator;
        private readonly OAuthParams _oauthParams;

        public InvoiceService(
            OAuthParams oauthParams,
            OAuthProfile oauthProfile, 
            InvoiceToBillTranslator invoiceToBillTranslator)
        {
            _oauthParams = oauthParams;
            _oauthProfile = oauthProfile;
            _invoiceToBillTranslator = invoiceToBillTranslator;
        }

        public void Save(Invoice invoice)
        {
            var oauthRequestValidator = new OAuthRequestValidator(
                _oauthProfile.AccessToken,
                _oauthProfile.AccessSecret,
                _oauthParams.ConsumerKey,
                _oauthParams.ConsumerSecret);
            var serviceContext = new ServiceContext(_oauthProfile.RealmId, IntuitServicesType.QBO, oauthRequestValidator);
            var dataService = new DataService(serviceContext);

            //var vendors = dataService.FindAll(new Vendor());

            var translate = _invoiceToBillTranslator.Translate(invoice);
            dataService.Add(translate);
        }
    }
}