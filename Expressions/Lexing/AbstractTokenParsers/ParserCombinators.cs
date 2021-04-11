using System.Collections.Generic;
using Expressions.Lexing.Tokens;

namespace Expressions.Lexing.AbstractTokenParsers
{ 
    public abstract class OptionalParser : ITokenParser
    {
        public record EmptyToken : ElementaryToken
        {
            public EmptyToken(Position start) : base("", start, start)
            {
            }
        }

        protected abstract ITokenParser Delegate { get; }

        public ParsingResult Parse(string text, Position initialPosition) =>
            Delegate.Parse(text, initialPosition) switch
            {
                SuccessfulParsingResult s => s,
                FailedParsingResult => new SuccessfulParsingResult(new EmptyToken(initialPosition))
            };
    }
    
    public abstract class SequentialParser : ITokenParser
    {
        protected abstract IEnumerable<ITokenParser> Parsers { get; }

        protected abstract Token ResultsToToken(List<Token> results);

        public ParsingResult Parse(string text, Position initialPosition)
        {
            var currentPosition = initialPosition;
            var results = new List<Token>();

            foreach (var parser in Parsers)
            {
                var result = parser.Parse(text, currentPosition);

                if (result is FailedParsingResult)
                {
                    return new FailedParsingResult();
                }

                var token = ((SuccessfulParsingResult) result).Token;

                results.Add(token);
                currentPosition = LexingUtils.UpdatePosition(text, token.End, token.End.AbsoluteOffset + 1);
            }
                
            return new SuccessfulParsingResult(ResultsToToken(results));
        }
    }
    public abstract class AlternativeParser : ITokenParser
    {
        protected abstract IEnumerable<ITokenParser> Parsers { get; }

        public ParsingResult Parse(string text, Position initialPosition)
        {
            foreach (var parser in Parsers)
            {
                switch (parser.Parse(text, initialPosition))
                {
                    case SuccessfulParsingResult s: return s;
                    case FailedParsingResult: continue;
                }
            }

            return new FailedParsingResult();
        }
    }
}
