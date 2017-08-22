using System;
using NLog;
using Spinvoice.Common.Domain.Pdf;
using Spinvoice.Infrastructure.Pdf;

namespace Spinvoice.Server.Services
{
    public class FileParseService : IFileParseService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IPdfParser _pdfParser;

        public FileParseService(IPdfParser pdfParser)
        {
            _pdfParser = pdfParser;
        }

        public PdfModel Parse(string filePath)
        {
            Logger.Info($"Start processing file {filePath}");
            try
            {
                var pdfModel = _pdfParser.Parse(filePath);
                Logger.Info($"Finished processing file {filePath}");
                return pdfModel;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }
    }
}
