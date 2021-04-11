using Expressions.Lexing.Tokens;

namespace Expressions.Lexing.AbstractTokenParsers
{
    public class SingleCharacterParser : ITokenParser
    {
        private readonly char _character;
        
        public SingleCharacterParser(char character)
        {
            _character = character;
        }
        
        public ParsingResult Parse(string text, Position initialPosition)
        {
            if (text[initialPosition.AbsoluteOffset] != _character)
            {
                return new FailedParsingResult();
            }
            
            var token = new SpecialSymbolToken(text.Substring(initialPosition.AbsoluteOffset, 1),
                initialPosition, initialPosition);
            return new SuccessfulParsingResult(token);
        }
    }
}