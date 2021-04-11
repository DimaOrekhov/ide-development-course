using System.Text.RegularExpressions;
using NUnit.Framework;

namespace ExpressionsTests.Lexing.TokenParsers
{
    public class NumberParserTests
    {
        [Test]
        public void Test1()
        {
            var text = "x + 30 * 40";
            var anyDigitSequence = new Regex(@"\G[0-9]+");
            var match1 = anyDigitSequence.Match(text, 4);
            var match2 = anyDigitSequence.Match(text, 7);
            var x = 2 + 2;
        }
    }
}