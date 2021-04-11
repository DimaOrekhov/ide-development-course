using System;
using System.Collections.Generic;
using Expressions.Lexing.Tokens;

namespace Expressions.Lexing.AbstractTokenParsers
{ 
    public class OptionalParser : ITokenParser
    {
        private readonly ITokenParser _parser;

        public static T ConvertOrNull<T>(Token token) where T : Token =>
            token is EmptyToken ? null : (T) token;
        
        public OptionalParser(ITokenParser parser)
        {
            _parser = parser;
        }
        
        public record EmptyToken : ElementaryToken
        {
            public EmptyToken(Position start) : base("", start, start)
            {
            }
        }
        
        public ParsingResult Parse(string text, Position initialPosition) =>
            _parser.Parse(text, initialPosition) switch
            {
                SuccessfulParsingResult s => s,
                FailedParsingResult => new SuccessfulParsingResult(new EmptyToken(initialPosition))
            };
    }
    
    public class ParseNothingParser : ITokenParser
    {
        public ParsingResult Parse(string text, Position initialPosition) => new SuccessfulParsingResult(new OptionalParser.EmptyToken(initialPosition));
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
                if (token is OptionalParser.EmptyToken)
                {
                    continue;
                }
                currentPosition = LexingUtils.UpdatePosition(text, token.End, token.End.AbsoluteOffset + 1);
            }
                
            return new SuccessfulParsingResult(ResultsToToken(results));
        }
    }
    public class AlternativeParser : ITokenParser
    {
        private readonly IEnumerable<ITokenParser> _parsers;

        public AlternativeParser(IEnumerable<ITokenParser> parsers)
        {
            _parsers = parsers;
        }
        
        public ParsingResult Parse(string text, Position initialPosition)
        {
            foreach (var parser in _parsers)
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

    public abstract class DecoratorParser : ITokenParser
    {
        private readonly ITokenParser _delegateParser;

        protected DecoratorParser(ITokenParser delegateParser)
        {
            _delegateParser = delegateParser;
        }

        protected abstract ParsingResult Decorate(ParsingResult result, string text);

        public ParsingResult Parse(string text, Position initialPosition)
            => Decorate(_delegateParser.Parse(text, initialPosition), text);
    }
}
