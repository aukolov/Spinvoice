﻿using System.Collections.Generic;
using iTextSharp.text.pdf.parser;
using Spinvoice.Common.Domain.Pdf;

namespace Spinvoice.Infrastructure.Pdf
{
    public interface ISmartTextExtractionStrategy : ITextExtractionStrategy
    {
        List<List<SentenceModel>> BlockSentences { get; }
    }
}