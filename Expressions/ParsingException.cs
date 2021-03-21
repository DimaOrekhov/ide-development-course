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
            new($"Unknown operator token: {token.Value}");

        public static ParsingException UnexpectedToken(Token token) =>
            new($"Unexpected token: {token.Value}");

        public static ParsingException UnexpectedEof() => new ("Unexpected EOF");
    }
}