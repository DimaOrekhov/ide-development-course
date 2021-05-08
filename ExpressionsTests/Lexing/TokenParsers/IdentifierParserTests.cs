using Expressions.Lexing;
using Expressions.Lexing.AbstractTokenParsers;
using Expressions.Lexing.TokenParsers;
using Expressions.Lexing.Tokens;
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
        }

        [Test]
        public void TestParseAtTheEnd()
        {
            Parser.ParseAndAssertNameAndPosition("2 * SCREAMING_SNAKE_CASE", Utils.OffsetFrom(InitialPosition, 4), 
                "SCREAMING_SNAKE_CASE");
            Parser.ParseAndAssertNameAndPosition("235 + _some_name", Utils.OffsetFrom(InitialPosition, 6), 
                "_some_name");
        }

        [Test]
        public void TestFailedParsing()
        {
            Parser.AssertParsingFails("+Hello");
            Parser.AssertParsingFails("#varow");
        }
    }
}