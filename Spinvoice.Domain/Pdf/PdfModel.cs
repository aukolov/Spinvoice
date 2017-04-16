using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Spinvoice.Domain.Pdf
{
    public class PdfModel
    {
        public PdfModel(List<PageModel> pages)
        {
            Pages = pages.AsReadOnly();
        }

        public ReadOnlyCollection<PageModel> Pages { get; private set; }
    }
}