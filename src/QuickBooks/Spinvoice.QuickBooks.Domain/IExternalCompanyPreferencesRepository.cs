using System;

namespace Spinvoice.QuickBooks.Domain
{
    public interface IExternalCompanyPreferencesRepository
    {
        string HomeCurrency { get; }
        event Action Updated;
    }
}