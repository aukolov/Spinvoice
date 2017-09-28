using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.Account
{
    public class ExternalAccount : IExternalAccount
    {
        public ExternalAccount(Intuit.Ipp.Data.Account account)
        {
            Id = account.Id;
            Name = account.Name;
        }

        public string Id { get; }
        public string Name { get; }
    }
}