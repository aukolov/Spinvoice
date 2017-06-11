using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Spinvoice.Domain.Accounting;
using Spinvoice.Utils;

// ReSharper disable once CheckNamespace
namespace Spinvoice.Domain.Pdf
{
    public class UnderAmountPositionStrategy : IPdfPositionAnalysisStrategy
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private string _amountHeaderText;
        private int _leftSentencesLength;
        private int? _nameIndex;
        private int? _quantityIndex;

        public Position[] GetValue(PdfModel pdfModel)
        {
            Logger.Info($"Start analyzing file {pdfModel.FileName}.");
            Logger.Info($"Amount header text: {_amountHeaderText}, left sentences length: {_leftSentencesLength}, " +
                        $"name index: {_nameIndex}, quantity index: {_quantityIndex}.");
            var positions = new List<Position>();

            foreach (var page in pdfModel.Pages)
            {
                Logger.Info("Checking page...");
                var amountHeaderSentence = page.Sentences.FirstOrDefault(model => model.Text == _amountHeaderText);
                if (amountHeaderSentence == null)
                {
                    Logger.Info("Amount header not found.");
                    continue;
                }

                foreach (var underAmountSentence in page.Below(amountHeaderSentence))
                {
                    decimal amount;
                    if (!AmountParser.TryParse(underAmountSentence.Text, out amount))
                    {
                        Logger.Info($"Unable to parse amount from '{underAmountSentence.Text}'.");
                        continue;
                    }
                    Logger.Info($"Amount parsed: {amount}.");

                    string name = null;
                    var quantity = 0;

                    if (_leftSentencesLength > 0)
                    {
                        var leftSentences = page.Left(underAmountSentence).ToArray();
                        Logger.Info($"Left sentences [{_leftSentencesLength}]: " +
                                    $"{string.Join(", ", leftSentences.Select(m => m.Text).ToArray())}.");

                        if (leftSentences.Length != _leftSentencesLength)
                        {
                            Logger.Info("Mismatch in count of left sentences.");
                            continue;
                        }

                        if (_nameIndex.HasValue)
                        {
                            name = leftSentences[_nameIndex.Value].Text;
                            Logger.Info($"Name found: {name}.");
                        }
                        if (_quantityIndex.HasValue)
                        {
                            var quantityText = leftSentences[_quantityIndex.Value].Text;
                            if (!int.TryParse(quantityText, out quantity))
                            {
                                Logger.Info($"Could not parse quantity: {quantityText}.");
                            }
                            else
                            {
                                Logger.Info($"Quantity found: {quantity}.");
                            }
                        }
                    }
                    positions.Add(new Position(name, quantity, amount));
                }
            }

            return positions.ToArray();
        }

        public bool Train(PdfModel pdfModel, RawPosition rawPosition)
        {
            Logger.Info($"Start training with raw position: {rawPosition}.");
            if (!rawPosition.IsFullyInitialized)
            {
                Logger.Info("Raw position is not fully initialized.");
            }

            var firstPage = pdfModel.Pages.FirstOrDefault();
            if (firstPage == null)
            {
                Logger.Info("First page not found.");
                return false;
            }
            var nameSentence = firstPage.Sentences.FirstOrDefault(m => rawPosition.Name == m.Text);
            if (nameSentence == null)
            {
                Logger.Info("Name sentence not found.");
                return false;
            }
            var amountSentence = firstPage.Sentences.FirstOrDefault(m =>
                m.Text == rawPosition.Amount
                && Math.Abs(nameSentence.Bottom - m.Bottom) < 5);
            if (amountSentence == null)
            {
                Logger.Info("Amount sentence not found.");
                return false;
            }

            var amountHeader = firstPage.Above(amountSentence);
            if (amountHeader == null)
            {
                Logger.Info("Amount header not found.");
                return false;
            }

            _amountHeaderText = amountHeader.Text;
            Logger.Info($"Amount header text: {_amountHeaderText}.");

            var leftSentences = firstPage.Left(amountSentence).ToArray();
            _leftSentencesLength = leftSentences.Length;
            Logger.Info($"Left sentences [{_leftSentencesLength}]: " +
                        $"{string.Join(", ", leftSentences.Select(m => m.Text).ToArray())}.");

            _nameIndex = leftSentences
                .Select((m, i) => new { m, i })
                .FirstOrDefault(x => x.m.Text == rawPosition.Name)
                ?.i;
            Logger.Info($"Name index: {_nameIndex}.");
            _quantityIndex = leftSentences
                .Select((m, i) => new { m, i })
                .FirstOrDefault(x => x.m.Text == rawPosition.Quantity)
                ?.i;
            Logger.Info($"Quantity index: {_quantityIndex}.");

            return true;
        }
    }
}