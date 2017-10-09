namespace Spinvoice.QuickBooks.Domain
{
    public interface IExternalCompany
    {
        string Id { get; }
        string Name { get; }
        Side Side { get; }
    }
}