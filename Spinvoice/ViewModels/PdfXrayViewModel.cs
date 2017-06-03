using System.Collections.Generic;
using System.Linq;
using Spinvoice.Domain.Pdf;

namespace Spinvoice.ViewModels
{
    public class PdfXrayViewModel
    {
        public PdfXrayViewModel(PdfModel pdfModel)
        {
            var pageModel = pdfModel.Pages.First();
            BlockModels = pageModel.Blocks;
        }

        public IReadOnlyList<BlockModel> BlockModels { get; }
    }
}
