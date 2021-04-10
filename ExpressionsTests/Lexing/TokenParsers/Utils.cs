using Expressions.Lexing;
using NUnit.Framework;

namespace ExpressionsTests.Lexing.TokenParsers
{
    public static class Utils
    {
        public static Position OffsetFrom(Position start, int offset) =>
            new(start.Line, start.Offset + offset, start.AbsoluteOffset + offset);
        
        public static void ParseAndAssertNameAndPosition(this ITokenParser parser, string text, Position start, string name)
        {
            var parsingResult = parser.Parse(text, start);
            Assert.IsInstanceOf(typeof(SuccessfulParsingResult), parsingResult);

            var token = ((SuccessfulParsingResult) parsingResult).Token;
            Assert.AreEqual(name, token.Value);
            Assert.AreEqual(start, token.Start);
            
            var end = OffsetFrom(start, name.Length - 1);
            Assert.AreEqual(end, token.End);
        }
    }
}