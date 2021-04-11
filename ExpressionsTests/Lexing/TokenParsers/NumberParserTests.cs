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
            var parser = NumberParser.UnsignedIntegerParser;
            parser.ParseAndAssertIsInstance("%01010", Utils.InitialPosition, typeof(UnsignedBinaryInteger));
            parser.ParseAndAssertIsInstance("&07020", Utils.InitialPosition, typeof(UnsignedOctalInteger));
            parser.ParseAndAssertIsInstance("$AF112", Utils.InitialPosition, typeof(UnsignedHexInteger));
            parser.ParseAndAssertIsInstance("12304", Utils.InitialPosition, typeof(UnsignedDecimalInteger));
        }

        [Test]
        public void Test2()
        {
            var parser = new NumberParser.UnsignedRealParser();
            var r1 = parser.Parse("10.12", Utils.InitialPosition);
            var r2 = parser.Parse("10e10", Utils.InitialPosition);
            var r3 = parser.Parse("10.11e10", Utils.InitialPosition);
            var r4 = parser.Parse("10.11e-10", Utils.InitialPosition);
            var x = 2;
        }
    }
}