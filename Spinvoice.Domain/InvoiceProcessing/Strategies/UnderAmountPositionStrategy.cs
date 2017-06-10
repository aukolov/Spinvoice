using System;
using System.Collections.Generic;
using System.Linq;
using Spinvoice.Domain.Accounting;
using Spinvoice.Domain.Pdf;
using Spinvoice.Utils;

namespace Spinvoice.Domain.InvoiceProcessing.Strategies
{
    public class UnderAmountPositionStrategy : IPdfPositionAnalysisStrategy
    {
        private string _amountHeaderText;
        private int _leftSentencesLength;
        private int? _nameIndex;
        private int? _quantityIndex;

        public Position[] GetValue(PdfModel pdfModel)
        {
            var positions = new List<Position>();

            foreach (var page in pdfModel.Pages)
            {
                var amountHeaderSentence = page.Sentences.FirstOrDefault(model => model.Text == _amountHeaderText);
                foreach (var underAmountSentence in page.Below(amountHeaderSentence))
                {
                    decimal amount;
                    if (AmountParser.TryParse(underAmountSentence.Text, out amount))
                    {
                        string name = null;
                        var quantity = 0;

                        if (_leftSentencesLength > 0)
                        {
                            var leftSentences = page.Left(underAmountSentence).ToArray();
                            if (leftSentences.Length == _leftSentencesLength)
                            {
                                if (_nameIndex.HasValue)
                                {
                                    name = leftSentences[_nameIndex.Value].Text;
                                }
                                if (_quantityIndex.HasValue)
                                {
                                    var quantityText = leftSentences[_quantityIndex.Value].Text;
                                    int.TryParse(quantityText, out quantity);
                                }
                            }
                        }
                        positions.Add(new Position(name, quantity, amount));
                    }
                }
            }

            return positions.ToArray();
        }

        public bool Train(PdfModel pdfModel, RawPosition rawPosition)
        {
            var firstPage = pdfModel.Pages.FirstOrDefault();
            // ReSharper disable once UseNullPropagation
            if (firstPage == null)
            {
                return false;
            }
            var nameSentence = firstPage.Sentences.FirstOrDefault(m => rawPosition.Name == m.Text);
            if (nameSentence == null)
            {
                return false;
            }
            var amountSentence = firstPage.Sentences.FirstOrDefault(m =>
                m.Text == rawPosition.Amount
                && Math.Abs(nameSentence.Bottom - m.Bottom) < 5);
            if (amountSentence == null)
            {
                return false;
            }

            var amountHeader = firstPage.Above(amountSentence);
            if (amountHeader == null)
            {
                return false;
            }

            _amountHeaderText = amountHeader.Text;

            var leftSentences = firstPage.Left(amountSentence).ToArray();
            _leftSentencesLength = leftSentences.Length;
            _nameIndex = leftSentences
                .Select((m, i) => new { m, i })
                .FirstOrDefault(x => x.m.Text == rawPosition.Name)
                ?.i;
            _quantityIndex = leftSentences
                .Select((m, i) => new { m, i })
                .FirstOrDefault(x => x.m.Text == rawPosition.Quantity)
                ?.i;
            
            return true;
        }
    }
}