using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Spinvoice.Domain.Pdf;

namespace Spinvoice.Tests.Domain.Pdf
{
    [TestFixture]
    public class PageModelTests
    {
        private static BlockModel CreateBlockModel(int blockNumber, params string[] sentences)
        {
            var sentencesList = new List<string>();
            sentencesList.AddRange(sentences);
            return new BlockModel(blockNumber, sentencesList);
        }

        private static PageModel CreatePageModel(int pageNumber, params BlockModel[] blockModels)
        {
            return new PageModel(pageNumber, blockModels.ToList());
        }

        private static void AssertLocation(Location location, int page, int block, int sentence)
        {
            Assert.AreEqual(page, location.Page, "Page");
            Assert.AreEqual(block, location.Block, "Block");
            Assert.AreEqual(sentence, location.Sentence, "Sentence");
        }

        [Test]
        public void MatchesFirstBlockFirstSentence()
        {
            var pageModel = CreatePageModel(0, CreateBlockModel(0, "abc", "aaa", "bbb"));

            var locationRanges = pageModel.Find("abc").ToArray();

            Assert.AreEqual(1, locationRanges.Length);
            var locationRange = locationRanges[0];
            AssertLocation(locationRange.Start, 0, 0, 0);
            AssertLocation(locationRange.End, 0, 0, 0);
        }

        [Test]
        public void MatchesFirstBlockSecondSentence()
        {
            var pageModel = CreatePageModel(0, CreateBlockModel(0, "ccc", "abc", "bbb"));

            var locationRanges = pageModel.Find("abc").ToArray();

            Assert.AreEqual(1, locationRanges.Length);
            var locationRange = locationRanges[0];
            AssertLocation(locationRange.Start, 0, 0, 1);
            AssertLocation(locationRange.End, 0, 0, 1);
        }

        [Test]
        public void MatchesSecondBlock()
        {
            var pageModel = CreatePageModel(0,
                CreateBlockModel(0, "ccc", "bbb"),
                CreateBlockModel(1, "ccc", "ddd", "abc", "bbb"));

            var locationRanges = pageModel.Find("abc").ToArray();

            Assert.AreEqual(1, locationRanges.Length);
            var locationRange = locationRanges[0];
            AssertLocation(locationRange.Start, 0, 1, 2);
            AssertLocation(locationRange.End, 0, 1, 2);
        }

        [Test]
        public void ReturnsCorrectPageNumber()
        {
            var pageModel = CreatePageModel(42, CreateBlockModel(0, "abc", "aaa", "bbb"));

            var locationRanges = pageModel.Find("abc").ToArray();

            Assert.AreEqual(1, locationRanges.Length);
            var locationRange = locationRanges[0];
            AssertLocation(locationRange.Start, 42, 0, 0);
            AssertLocation(locationRange.End, 42, 0, 0);
        }

        [Test]
        public void MatchesTwoSentences()
        {
            var pageModel = CreatePageModel(0, CreateBlockModel(0, "abc", "aaa", "bbb"));

            var locationRanges = pageModel.Find("aaa bbb").ToArray();

            Assert.AreEqual(1, locationRanges.Length);
            var locationRange = locationRanges[0];
            AssertLocation(locationRange.Start, 0, 0, 1);
            AssertLocation(locationRange.End, 0, 0, 2);
        }

        [Test]
        public void MatchesSentenceIfStartsFromSpaces()
        {
            var pageModel = CreatePageModel(0, CreateBlockModel(0, "abc", "   aaa", "bbb"));

            var locationRanges = pageModel.Find("aaa").ToArray();

            Assert.AreEqual(1, locationRanges.Length);
            var locationRange = locationRanges[0];
            AssertLocation(locationRange.Start, 0, 0, 1);
            AssertLocation(locationRange.End, 0, 0, 1);
        }

        [Test]
        public void DoesNotMatcheSentenceIfContinuesWithNonWhitespace()
        {
            var pageModel = CreatePageModel(0, CreateBlockModel(0, "abc", "aaab", "bbb"));

            var locationRanges = pageModel.Find("aaa").ToArray();

            Assert.AreEqual(0, locationRanges.Length);
        }


        [Test]
        public void MatchesSentenceIfEndsWithSpaces()
        {
            var pageModel = CreatePageModel(0, CreateBlockModel(0, "abc", "aaa   ", "bbb"));

            var locationRanges = pageModel.Find("aaa").ToArray();

            Assert.AreEqual(1, locationRanges.Length);
            var locationRange = locationRanges[0];
            AssertLocation(locationRange.Start, 0, 0, 1);
            AssertLocation(locationRange.End, 0, 0, 1);
        }
    }
}