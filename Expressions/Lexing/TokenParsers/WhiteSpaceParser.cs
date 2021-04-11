using System;
using Expressions.Lexing.AbstractTokenParsers;

namespace Expressions.Lexing.TokenParsers
{
    public record WhiteSpaceToken : Token
    {
        public WhiteSpaceToken(string value, Position start, Position end) : base(value, start, end)
        {
        }
    }

    public class WhiteSpaceParser : PredicateTokenParser
    {
        protected override Predicate<char> Predicate => char.IsWhiteSpace;
        
        protected override Token MatchedSymbolsToToken(string match, Position start, Position end)
        {
            throw new NotImplementedException();
        }
    }
}
