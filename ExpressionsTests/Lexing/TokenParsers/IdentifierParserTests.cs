using Expressions.Lexing;
using Expressions.Lexing.TokenParsers;
using NUnit.Framework;

namespace ExpressionsTests.Lexing.TokenParsers
{
    public class IdentifierParserTests
    {
        private static readonly ITokenParser Parser = new IdentifierParser();
        private static readonly Position InitialPosition = new(0, 0, 0);

        [Test]
        public void Test1()
        {
            Parser.ParseAndAssertNameAndPosition("abc + x", InitialPosition, "abc");

            var xPosition = Utils.OffsetFrom(InitialPosition, 6);
            Parser.ParseAndAssertNameAndPosition("abc + x", xPosition, "x");
            Parser.ParseAndAssertNameAndPosition("_property + 2", InitialPosition, "_property");
            Parser.ParseAndAssertNameAndPosition("snake_case * 3 - 4", InitialPosition, "snake_case");
            Parser.ParseAndAssertNameAndPosition("camelCase - 12", InitialPosition, "camelCase");
            Parser.ParseAndAssertNameAndPosition("CapitalCamelCase- 130", InitialPosition, "CapitalCamelCase");
            Parser.ParseAndAssertNameAndPosition("2 * SCREAMING_SNAKE_CASE", Utils.OffsetFrom(InitialPosition, 4), 
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