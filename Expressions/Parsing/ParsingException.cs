using System;
using Expressions.Lexing;
using Expressions.Lexing.Tokens;

namespace Expressions.Parsing
{
    public class ParsingException : Exception
    {
        private ParsingException(string message) : base(message)
        {
        }

        public static ParsingException UnknownTokenType(ElementaryToken token) => 
            new ($"Unknown token type: {token.GetType().Name}");

        public static ParsingException UnknownOperatorToken(ElementaryToken token) =>
            new($"Unknown operator token {token.Value} at position {token.Start}");

        public static ParsingException UnexpectedToken(ElementaryToken token) =>
            new($"Unexpected token: {token.Value} at position {token.Start}");

        public static ParsingException UnexpectedEof() => new ("Unexpected EOF");

        public static ParsingException ClosingBracketExpected() => new(") expected");
    }
}