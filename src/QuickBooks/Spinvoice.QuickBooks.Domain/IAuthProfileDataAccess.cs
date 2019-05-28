namespace Spinvoice.QuickBooks.Domain
{
    public interface IAuthProfileDataAccess
    {
        AuthProfile[] GetAll();
        void AddOrUpdate(AuthProfile entity);
        void DeleteAll();

    }
}