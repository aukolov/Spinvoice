namespace Spinvoice.Application.Services
{
    public interface IExchangeRatesLoader
    {
        void Load(string filePath);
    }
}