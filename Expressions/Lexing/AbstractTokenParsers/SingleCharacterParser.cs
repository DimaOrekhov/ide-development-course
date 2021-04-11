using System;
using Expressions.Lexing.Tokens;

namespace Expressions.Lexing.AbstractTokenParsers
{
    public class SingleCharacterParser : ITokenParser
    {
        public delegate Token ToTokenConverter(string value, Position position);

        private static readonly ToTokenConverter DefaultConverter =
            (text, pos) => new SpecialSymbolToken(text, pos, pos);
        
        private readonly char _character;
        private readonly ToTokenConverter _toTokenConverter;
        
        public SingleCharacterParser(char character, ToTokenConverter toTokenToTokenConverter = null)
        {
            _character = character;
            _toTokenConverter = toTokenToTokenConverter ?? DefaultConverter;
        }
        
        public ParsingResult Parse(string text, Position initialPosition)
        {
            if (initialPosition.AbsoluteOffset >= text.Length ||
                text[initialPosition.AbsoluteOffset] != _character)
            {
                return new FailedParsingResult();
            }

            var token = _toTokenConverter(text[initialPosition.AbsoluteOffset].ToString(), initialPosition);
            return new SuccessfulParsingResult(token);
        }
    }
}