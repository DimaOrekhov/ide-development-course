using System;
using System.Collections.Generic;
using Expressions.Lexing.AbstractTokenParsers;
using Expressions.Lexing.Tokens;

namespace Expressions.Lexing.TokenParsers
{ 
    public class UnsignedIntegerParser : AlternativeParser
    {
        protected override IEnumerable<ITokenParser> Parsers => new List<ITokenParser>
            {new BinaryIntegerParser(), new OctalIntegerParser(), new DecimalIntegerParser(), new HexIntegerParser()};
    }

    public class BinaryIntegerParser : SequentialParser
    {
        protected override IEnumerable<ITokenParser> Parsers => new List<ITokenParser> 
            {new SingleCharacterParser('%'), new BinaryDigitSequenceParser()};

        protected override Token ResultsToToken(List<Token> results) => 
            new UnsignedBinaryInteger(new PercentToken(results[0].Start), (BinaryDigitSequence) results[1]);
    }
    
    public class BinaryDigitSequenceParser : PredicateTokenParser
    {
        protected override Predicate<char> Predicate => c => c == '0' || c == '1';

        protected override ElementaryToken MatchedSymbolsToToken(string match, Position start, Position end)
            => new BinaryDigitSequence(match, start, end);
    }

    public class DecimalIntegerParser : ITokenParser // TODO: add abstract decorator parser
    {
        private readonly DecimalDigitSequenceParser _parser = new();
        
        public ParsingResult Parse(string text, Position initialPosition)
        {
            return _parser.Parse(text, initialPosition) switch
            {
                SuccessfulParsingResult s => new SuccessfulParsingResult(
                    new UnsignedDecimalInteger((DecimalDigitSequence) s.Token)),
                FailedParsingResult f => f
            };
        }
    }
    
    public class DecimalDigitSequenceParser : PredicateTokenParser
    {
        protected override Predicate<char> Predicate => char.IsDigit;

        protected override ElementaryToken MatchedSymbolsToToken(string match, Position start, Position end) =>
            new DecimalDigitSequence(match, start, end);
    }

    public class OctalIntegerParser : SequentialParser
    {
        protected override IEnumerable<ITokenParser> Parsers => new List<ITokenParser> 
            {new SingleCharacterParser('&'), new OctalDigitSequenceParser()};

        protected override Token ResultsToToken(List<Token> results) => 
            new UnsignedOctalInteger(new AmpersandToken(results[0].Start), (OctalDigitSequence) results[1]);
    }
    
    public class OctalDigitSequenceParser : PredicateTokenParser
    {
        protected override Predicate<char> Predicate => IsHexDigit;

        private static bool IsHexDigit(char obj) => obj >= '0' && obj <= '7';

        protected override ElementaryToken MatchedSymbolsToToken(string match, Position start, Position end) =>
            new OctalDigitSequence(match, start, end);
    }

    public class HexIntegerParser : SequentialParser
    {
        protected override IEnumerable<ITokenParser> Parsers => new List<ITokenParser> 
            {new SingleCharacterParser('$'), new HexDigitSequenceParser()};

        protected override Token ResultsToToken(List<Token> results) => 
            new UnsignedHexInteger(new DollarSignToken(results[0].Start), (HexDigitSequence) results[1]);
    }
    
    public class HexDigitSequenceParser : PredicateTokenParser
    {
        private static bool IsHexDigit(char obj) =>
            char.IsDigit(obj)
            || obj >= 'A' && obj <= 'F'
            || obj >= 'a' && obj <= 'f';
        
        protected override Predicate<char> Predicate => IsHexDigit;

        protected override ElementaryToken MatchedSymbolsToToken(string match, Position start, Position end) =>
            new HexDigitSequence(match, start, end);
    }
}