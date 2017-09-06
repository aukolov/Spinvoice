using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Spinvoice.Common.Domain.Pdf;
using Spinvoice.Domain.Accounting;
using Spinvoice.Utils;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable once CheckNamespace
namespace Spinvoice.Domain.Pdf
{
    public class UnderAmountPositionStrategy : IPdfPositionAnalysisStrategy
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public string AmountHeaderText { get; private set; }
        public int LeftSentencesLength { get; private set; }
        public int? NameIndex { get; private set; }
        public int? QuantityIndex { get; private set; }

        public Position[] GetValue(PdfModel pdfModel)
        {
            Logger.Trace($"Start analyzing file {pdfModel.FileName}.");
            Logger.Trace($"Amount header text: {AmountHeaderText}, left sentences length: {LeftSentencesLength}, " +
                        $"name index: {NameIndex}, quantity index: {QuantityIndex}.");
            var positions = new List<Position>();

            foreach (var page in pdfModel.Pages)
            {
                Logger.Trace("Checking page...");
                var amountHeaderSentence = page.Sentences.FirstOrDefault(model => model.Text == AmountHeaderText);
                if (amountHeaderSentence == null)
                {
                    Logger.Trace("Amount header not found.");
                    continue;
                }

                foreach (var underAmountSentence in page.Below(amountHeaderSentence))
                {
                    decimal amount;
                    if (!AmountParser.TryParse(underAmountSentence.Text, out amount))
                    {
                        Logger.Trace($"Unable to parse amount from '{underAmountSentence.Text}'.");
                        continue;
                    }
                    Logger.Trace($"Amount parsed: {amount}.");

                    string name = null;
                    var quantity = 0;

                    if (LeftSentencesLength > 0)
                    {
                        var leftSentences = page.Left(underAmountSentence).ToArray();
                        Logger.Trace($"Left sentences [{leftSentences.Length}]: " +
                                    $"{string.Join(", ", leftSentences.Select(m => m.Text).ToArray())}.");

                        if (leftSentences.Length != LeftSentencesLength)
                        {
                            Logger.Trace("Mismatch in count of left sentences.");
                            continue;
                        }

                        if (NameIndex.HasValue)
                        {
                            name = leftSentences[NameIndex.Value].Text;
                            Logger.Trace($"Name found: {name}.");
                        }
                        if (QuantityIndex.HasValue)
                        {
                            var quantityText = leftSentences[QuantityIndex.Value].Text;
                            if (!QuantityParser.TryParse(quantityText, out quantity))
                            {
                                Logger.Trace($"Could not parse quantity: {quantityText}.");
                            }
                            else
                            {
                                Logger.Trace($"Quantity found: {quantity}.");
                            }
                        }
                    }
                    positions.Add(new Position(name, quantity, amount));
                }
            }
            Logger.Trace($"Positions found: {positions.Count}");

            return positions.ToArray();
        }

        public bool Train(PdfModel pdfModel, RawPosition rawPosition)
        {
            Logger.Trace($"Start training with raw position: {rawPosition}.");
            if (!rawPosition.IsFullyInitialized)
            {
                Logger.Trace("Raw position is not fully initialized.");
            }

            foreach (var page in pdfModel.Pages.Take(3))
            {
                if (Train(page, rawPosition))
                {
                    return true;
                }
            }
            return false;
        }

        private bool Train(PageModel page, RawPosition rawPosition)
        {
            if (page == null)
            {
                Logger.Trace("Page not found.");
                return false;
            }
            var nameSentence = page.Sentences.FirstOrDefault(m => rawPosition.Name == m.Text);
            if (nameSentence == null)
            {
                Logger.Trace("Name sentence not found.");
                return false;
            }
            var amountSentence = page.Sentences.FirstOrDefault(m =>
                m.Text == rawPosition.Amount
                && Math.Abs(nameSentence.Bottom - m.Bottom) < 5);
            if (amountSentence == null)
            {
                Logger.Trace("Amount sentence not found.");
                return false;
            }

            var amountHeader = page.Above(amountSentence);
            if (amountHeader == null)
            {
                Logger.Trace("Amount header not found.");
                return false;
            }

            AmountHeaderText = amountHeader.Text;
            Logger.Trace($"Amount header text: {AmountHeaderText}.");

            var leftSentences = page.Left(amountSentence).ToArray();
            LeftSentencesLength = leftSentences.Length;
            Logger.Trace($"Left sentences [{LeftSentencesLength}]: " +
                        $"{string.Join(", ", leftSentences.Select(m => m.Text).ToArray())}.");

            NameIndex = leftSentences
                .Select((m, i) => new { m, i })
                .FirstOrDefault(x => x.m.Text == rawPosition.Name)
                ?.i;
            Logger.Trace($"Name index: {NameIndex}.");
            QuantityIndex = leftSentences
                .Select((m, i) => new { m, i })
                .FirstOrDefault(x => x.m.Text == rawPosition.Quantity)
                ?.i;
            Logger.Trace($"Quantity index: {QuantityIndex}.");

            return true;
        }
    }
}