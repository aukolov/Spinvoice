﻿using System.Linq;
using Spinvoice.Domain.Pdf;

namespace Spinvoice.ViewModels
{
    public class PdfXrayViewModel
    {
        public PdfXrayViewModel(PdfModel pdfModel)
        {
            var pageModel = pdfModel.Pages.First();
            Sentences = pageModel.Sentences.ToArray();
        }

        public SentenceModel[] Sentences { get; }
    }
}
