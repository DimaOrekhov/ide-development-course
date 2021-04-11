using Expressions.Lexing;
using Expressions.Lexing.TokenParsers;
using NUnit.Framework;

namespace ExpressionsTests.Lexing.TokenParsers
{
    public class NumberParserTests
    {
        [Test]
        public void Test1()
        {
            var parser = new UnsignedIntegerParser();
            parser.ParseAndAssertIsInstance("%01010", Utils.InitialPosition, typeof(UnsignedBinaryInteger));
            parser.ParseAndAssertIsInstance("&07020", Utils.InitialPosition, typeof(UnsignedOctalInteger));
            parser.ParseAndAssertIsInstance("$AF112", Utils.InitialPosition, typeof(UnsignedHexInteger));
            parser.ParseAndAssertIsInstance("12304", Utils.InitialPosition, typeof(UnsignedDecimalInteger));
        }
    }
}