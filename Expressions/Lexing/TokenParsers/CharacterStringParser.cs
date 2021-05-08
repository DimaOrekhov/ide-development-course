using System;
using System.Collections.Generic;
using System.Linq;
using Expressions.Lexing.AbstractTokenParsers;
using Expressions.Lexing.Tokens;

namespace Expressions.Lexing.TokenParsers
{
    public static class CharacterStringParser
    {
        private static readonly ITokenParser QuotedOrControlParser = new AlternativeParser(new QuotedStringParser(), new ControlStringParser());

        public static readonly ITokenParser Instance = new ManyParser(QuotedOrControlParser,
            tokens =>
            {
                var elements = tokens.Cast<CharacterStringElementToken>().ToList();
                var token = new CharacterStringToken(elements);
                return new SuccessfulParsingResult(token);
            });
    }
    
    public class QuotedStringParser : SequentialParser
    {
        private static readonly ITokenParser QuoteParser =
            new SingleCharacterParser('\'', (_, position) => new SingleQuoteToken(position));
        
        private static readonly List<ITokenParser> _parsers = new List<ITokenParser>
        {
            QuoteParser, new OptionalParser(new StringCharacterParser()), QuoteParser
        };
        
        protected override IEnumerable<ITokenParser> Parsers => _parsers;
        
        protected override Token ResultsToToken(List<Token> results)
        {
            var start = results[0].Start;
            var end = results[^1].End;
            return new QuotedStringToken(start, end);
        }
    }

    public class StringCharacterParser : PredicateTokenParser
    {
        protected override Predicate<char> Predicate => IsStringCharacter;

        private static bool IsStringCharacter(char character) => character != '\'' && character != '#';

        protected override Token MatchedSymbolsToToken(string match, Position start, Position end) =>
            new StringBodyToken(start, end);
    }

    public class ControlStringParser : SequentialParser
    {
        private static readonly List<ITokenParser> _parsers = new List<ITokenParser>
        {
            new SingleCharacterParser('#', (_, position) => new HashToken(position)),
            NumberParser.UnsignedIntegerParser
        };

        protected override IEnumerable<ITokenParser> Parsers => _parsers;

        protected override Token ResultsToToken(List<Token> results) =>
            new ControlStringToken((HashToken) results[0], (UnsignedIntegerToken) results[^1]);
    }
}