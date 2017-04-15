using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Spinvoice.Domain.Utils
{
    public class TextDecoder
    {
        public static string Decode(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            if (text.Any(IsSurrogate))
            {
                return new string(Decode(text.ToCharArray()).ToArray());

            }
            return text;
        }

        private static bool IsSurrogate(char c)
        {
            return c >= 0xDB80 && c <= 0xDBFF;
        }

        private static IEnumerable<char> Decode(char[] chars)
        {
            var i = 0;
            while (i < chars.Length)
            {
                var c = chars[i];
                if (IsSurrogate(c) && i + 1 < chars.Length)
                {
                    yield return DecodePair(c, chars[i + 1]);
                    i += 2;
                }
                else
                {
                    yield return c;
                    i++;
                }
            }
        }

        private static char DecodePair(int hi, int lo)
        {
            if (hi == 56256)
            {
                return (char)(lo - 56256 - 35);
            }
            else
            {
                return (char)((hi - 0xD800) * 0x400 + (lo - 0xDC00) + 0x10000);
            }
        }
    }
}