using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Spinvoice.Domain.Utils;

namespace Spinvoice.Tests.Domain.Utils
{
    [TestFixture()]
    public class TextDecoderTests
    {
        [Test]
        public void DecodesUtf16String()
        {
            var chars = new[]
            {
                56256, 56356, 56256, 56370, 32, 56256, 56374, 56256, 56358, 56256, 56363, 56256, 56360, 56256, 56369,
                56256, 56366, 56256, 56360, 56256, 56373
            }.Select(i => (char)i).ToArray();
            var text = new string(chars);

            var decodedText = TextDecoder.Decode(text);

            Assert.AreEqual("AO SCHENKER", decodedText);
        }
    }
}