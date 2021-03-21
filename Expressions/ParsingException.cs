using System;

namespace Expressions
{
    public class ParsingException : Exception
    {
        private ParsingException(string message) : base(message)
        {
        }

        public static ParsingException UnknownTokenType(Token token) => 
            new ($"Unknown token type: {token.GetType().Name}");

        public static ParsingException UnknownOperatorToken(Token token) =>
            new($"Unknown operator token {token.Value} at position {token.Position}");

        public static ParsingException UnexpectedToken(Token token) =>
            new($"Unexpected token: {token.Value} at position {token.Position}");

        public static ParsingException UnexpectedEof() => new ("Unexpected EOF");

        public static ParsingException ClosingBracketExpected() => new(") expected");
    }
}