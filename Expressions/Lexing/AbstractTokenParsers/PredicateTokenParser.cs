using System;
using Expressions.Lexing.Tokens;

namespace Expressions.Lexing.AbstractTokenParsers
{
    public abstract class PredicateTokenParser : ITokenParser
    {
        protected abstract Predicate<char> Predicate
        {
            get;
        }

        protected abstract ElementaryToken MatchedSymbolsToToken(string match, Position start, Position end);
        
        public ParsingResult Parse(string text, Position initialPosition)
        {
            var nSymbolsMatched = 0;
            for (var i = initialPosition.AbsoluteOffset; i < text.Length; i++)
            {
                if (!Predicate(text[i]))
                {
                    break;
                }
                nSymbolsMatched++;
            }

            if (nSymbolsMatched == 0)
            {
                return new FailedParsingResult();
            }

            var endPosition = LexingUtils.UpdatePosition(text, initialPosition, initialPosition.AbsoluteOffset + nSymbolsMatched - 1);
            var token = MatchedSymbolsToToken(text.Substring(initialPosition.AbsoluteOffset, nSymbolsMatched), initialPosition, endPosition);
            return new SuccessfulParsingResult(token);
        }
    }
}