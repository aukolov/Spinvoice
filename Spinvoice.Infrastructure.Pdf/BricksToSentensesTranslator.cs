using System;
using System.Collections.Generic;
using Spinvoice.Domain.Pdf;

namespace Spinvoice.Infrastructure.Pdf
{
    internal class BricksToSentensesTranslator
    {
        public List<List<SentenceModel>> Translate(IEnumerable<IBrick[]> brickLists)
        {
            var blockSentences = new List<List<SentenceModel>>();
            foreach (var brickList in brickLists)
            {
                var sentences = new List<SentenceModel>();
                var builder = new SentenceModelBuilder();

                for (var i = 0; i < brickList.Length; i++)
                {
                    var brick = brickList[i];
                    if (builder.IsEmpty)
                    {
                        Append(builder, brick);
                    }
                    else
                    {
                        var prevBrick = brickList[i - 1];
                        if (RoughEqual(brick.Y, prevBrick.Y)
                            && RoughEqual(prevBrick.X + prevBrick.Width, brick.X))
                        {
                            Append(builder, brick);
                        }
                        else
                        {
                            sentences.Add(builder.Build());
                            builder = new SentenceModelBuilder();
                            Append(builder, brick);
                        }
                    }
                    if (i == brickList.Length - 1)
                    {
                        sentences.Add(builder.Build());
                    }
                }
                blockSentences.Add(sentences);
            }
            return blockSentences;
        }

        private static void Append(SentenceModelBuilder builder, IBrick brick)
        {
            builder.Append(
                brick.Text,
                brick.X,
                brick.Y,
                brick.Width,
                brick.Height);
        }

        private static bool RoughEqual(double currentRectY, double prevRectX)
        {
            return Math.Abs(currentRectY - prevRectX) < 1;
        }
    }
}