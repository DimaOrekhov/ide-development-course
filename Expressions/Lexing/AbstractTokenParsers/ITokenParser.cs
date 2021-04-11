using Expressions.Lexing.Tokens;

namespace Expressions.Lexing.AbstractTokenParsers
{
    public interface ITokenParser
    {
        ParsingResult Parse(string text, Position initialPosition);
    }

    public class ParsingResult
    {
    }

    public class SuccessfulParsingResult : ParsingResult
    {
        public readonly Token Token;

        public SuccessfulParsingResult(Token token)
        {
            Token = token;
        }
    }

    public class FailedParsingResult : ParsingResult
    {
    }
}