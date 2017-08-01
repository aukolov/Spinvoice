namespace Spinvoice.Services
{
    public interface IExchangeRatesLoader
    {
        void Load(string filePath);
    }
}