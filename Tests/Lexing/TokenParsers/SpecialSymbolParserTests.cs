using Expressions.Lexing;
using Expressions.Lexing.AbstractTokenParsers;
using Expressions.Lexing.TokenParsers;
using Expressions.Lexing.Tokens;
using NUnit.Framework;

namespace ExpressionsTests.Lexing.TokenParsers
{
    public class SpecialSymbolParserTests
    {
        private static readonly ITokenParser Parser = new SpecialSymbolParser();
        private static readonly Position InitialPosition = new(0, 0, 0);
        
        [Test]
        public void Test1()
        {
            Parser.ParseAndAssertNameAndPosition("<< x", InitialPosition, "<<");
            Parser.ParseAndAssertNameAndPosition("< x", InitialPosition, "<");
            Parser.ParseAndAssertNameAndPosition("x += 2", Utils.OffsetFrom(InitialPosition, 2), "+=");
            Parser.ParseAndAssertNameAndPosition("x + 2", Utils.OffsetFrom(InitialPosition, 2), "+");
        }
    }
}