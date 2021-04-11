using System;
using Expressions.Lexing.AbstractTokenParsers;
using Expressions.Lexing.Tokens;

namespace Expressions.Lexing.TokenParsers
{
    public record WhiteSpaceToken : ElementaryToken
    {
        public WhiteSpaceToken(string value, Position start, Position end) : base(value, start, end)
        {
        }
    }

    public class WhiteSpaceParser : PredicateTokenParser
    {
        protected override Predicate<char> Predicate => char.IsWhiteSpace;
        
        protected override ElementaryToken MatchedSymbolsToToken(string match, Position start, Position end)
        {
            throw new NotImplementedException();
        }
    }
}
