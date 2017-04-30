namespace Spinvoice.IntegrationTests
{
    public class TestInput
    {
        public string PdfPath { get; }
        public string JsonPath { get; }

        public TestInput(string pdfPath, string jsonPath)
        {
            PdfPath = pdfPath;
            JsonPath = jsonPath;
        }
    }
}