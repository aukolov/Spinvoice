using System;
using System.Linq;
using Intuit.Ipp.Data;
using Spinvoice.QuickBooks.Connection;
using Spinvoice.QuickBooks.Domain;
using Spinvoice.Utils;

namespace Spinvoice.QuickBooks.Company
{
    public class ExternalCompanyPreferencesRepository : IExternalCompanyPreferencesRepository
    {
        private string _homeCurrency;
        private readonly IExternalConnection _externalConnection;

        public ExternalCompanyPreferencesRepository(IExternalConnection externalConnection)
        {
            _externalConnection = externalConnection;
            externalConnection.Connected += LoadHomeCurrency;
            if (externalConnection.IsConnected)
            {
                LoadHomeCurrency();
            }
        }

        public event Action Updated;

        private void LoadHomeCurrency()
        {
            _homeCurrency = _externalConnection.GetAll<Preferences>().Single().CurrencyPrefs.HomeCurrency.Value;
            Updated.Raise();
        }

        public string HomeCurrency
        {
            get
            {
                if (_homeCurrency == null)
                {
                    LoadHomeCurrency();
                }
                return _homeCurrency;
            }
        }
    }
}