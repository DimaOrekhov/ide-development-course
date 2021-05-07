using System;
using Expressions.Lexing;
using Expressions.Lexing.AbstractTokenParsers;
using Expressions.Lexing.TokenParsers;
using Expressions.Lexing.Tokens;
using NUnit.Framework;

namespace ExpressionsTests.Lexing.TokenParsers
{
    public static class Utils
    {
        public static readonly Position InitialPosition = new Position(0, 0, 0);
        
        public static Position OffsetFrom(Position start, int offset) =>
            new(start.Line, start.Offset + offset, start.AbsoluteOffset + offset);
        
        public static void ParseAndAssertNameAndPosition(this ITokenParser parser, string text, Position start, string name)
        {
            var parsingResult = parser.Parse(text, start);
            Assert.IsInstanceOf(typeof(SuccessfulParsingResult), parsingResult);

            var token = (ElementaryToken)((SuccessfulParsingResult) parsingResult).Token;
            Assert.AreEqual(name, token.Value);
            Assert.AreEqual(start, token.Start);
            
            var end = OffsetFrom(start, name.Length - 1);
            Assert.AreEqual(end, token.End);
        }

        public static T ParseToken<T>(this ITokenParser parser, string text, Position start) where T : Token
        {
            var parsingResult = parser.Parse(text, start);
            Assert.IsInstanceOf<SuccessfulParsingResult>(parsingResult);

            var token = ((SuccessfulParsingResult) parsingResult).Token;
            Assert.IsInstanceOf<T>(token);
            return (T) token;
        }
    }
}