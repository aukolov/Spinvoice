using Spinvoice.Infrastructure.Pdf;

namespace Spinvoice.IntegrationTests.Mocks
{
    public class TesseractDataPathProviderMock : ITesseractDataPathProvider
    {
        public string Path => @"C:\Projects\my\Spinvoice\Spinvoice.Infrastructure.Pdf\tessdata";
    }
}