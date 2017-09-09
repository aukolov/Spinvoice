// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Linq;
using Spinvoice.Common.Domain.Pdf;
using Spinvoice.Domain.Pdf;
using Spinvoice.Utils;

namespace Spinvoice.Domain.InvoiceProcessing.Strategies
{
    public class LocationStrategy : IStringPdfAnalysisStrategy
    {
        public double Left { get; private set; }
        public double Top { get; private set; }
        public int PageIndex { get; private set; }

        public string GetValue(PdfModel pdfModel)
        {
            if (PageIndex >= pdfModel.Pages.Count)
            {
                return null;
            }
            var page = pdfModel.Pages[PageIndex];
            if (!page.Sentences.Any())
            {
                return null;
            }
            var sentence = page.Sentences.MinBy(CalculateDistance);
            return sentence.Text.Trim();
        }

        private double CalculateDistance(SentenceModel sentence)
        {
            var d = Math.Sqrt(Math.Pow(sentence.Top - Top, 2))
                    + Math.Sqrt(Math.Pow(sentence.Left - Left, 2));
            return d;
        }

        public bool Train(PdfModel pdfModel, string value)
        {
            SentenceModel sentence = null;
            PageModel page = null;

            foreach (var candidatePage in pdfModel.Pages)
            {
                sentence = candidatePage.Sentences
                    .FirstOrDefault(model => model.Text.Trim() == value.Trim());
                if (sentence != null)
                {
                    page = candidatePage;
                    break;
                }
            }
            if (sentence == null)
            {
                return false;
            }

            PageIndex = page.PageNumber;
            Left = sentence.Left;
            Top = sentence.Top;

            return true;
        }
    }
}