using System.Collections.Generic;
using Spinvoice.Common.Domain.Pdf;
using Spinvoice.Domain.Pdf;

namespace Spinvoice.Infrastructure.Pdf
{
    internal interface IBricksToSentensesTranslator
    {
        List<List<SentenceModel>> Translate(IEnumerable<IBrick[]> brickLists);
    }
}