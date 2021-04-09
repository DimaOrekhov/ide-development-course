using Expressions.Lexing;
using Expressions.Lexing.TokenParsers;
using NUnit.Framework;

namespace ExpressionsTests.Lexing.TokenParsers
{
    public class IdentifierParserTests
    {
        private static readonly ITokenParser Parser = new IdentifierParser();
        private static readonly Position InitialPosition = new Position(0, 0, 0);
        
        private static Position OffsetFrom(Position start, int offset) =>
            new(start.Line, start.Offset + offset, start.AbsoluteOffset + offset);

        private static void ParseAndAssertIdentifierAndPosition(string text, Position start, string name)
        {
            var parsingResult = Parser.Parse(text, start);
            Assert.IsInstanceOf(typeof(SuccessfulParsingResult), parsingResult);

            var token = ((SuccessfulParsingResult) parsingResult).Token;
            Assert.AreEqual(name, token.Value);
            Assert.AreEqual(start, token.Start);
            
            var end = OffsetFrom(start, name.Length - 1);
            Assert.AreEqual(end, token.End);
        }
        
        [Test]
        public void Test1()
        {
            ParseAndAssertIdentifierAndPosition("abc + x", InitialPosition, "abc");

            var xPosition = OffsetFrom(InitialPosition, 6);
            ParseAndAssertIdentifierAndPosition("abc + x", xPosition, "x");
            ParseAndAssertIdentifierAndPosition("_property + 2", InitialPosition, "_property");
            ParseAndAssertIdentifierAndPosition("snake_case * 3 - 4", InitialPosition, "snake_case");
            ParseAndAssertIdentifierAndPosition("camelCase - 12", InitialPosition, "camelCase");
            ParseAndAssertIdentifierAndPosition("CapitalCamelCase- 130", InitialPosition, "CapitalCamelCase");
            ParseAndAssertIdentifierAndPosition("2 * SCREAMING_SNAKE_CASE", OffsetFrom(InitialPosition, 4), 
                "SCREAMING_SNAKE_CASE");
        }

        [Test]
        public void TestParseInTheMiddle()
        {
            
        }

        [Test]
        public void TestParseAtTheEnd()
        {
            
        }

        [Test]
        public void TestFailedParsing()
        {
            
        }
    }
}