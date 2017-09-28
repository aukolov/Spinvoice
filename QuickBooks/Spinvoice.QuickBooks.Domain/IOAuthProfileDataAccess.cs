namespace Spinvoice.QuickBooks.Domain
{
    public interface IOAuthProfileDataAccess
    {
        OAuthProfile[] GetAll();
        void AddOrUpdate(OAuthProfile entity);
        void DeleteAll();

    }
}