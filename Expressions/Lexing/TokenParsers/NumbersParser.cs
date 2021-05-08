using System;
using System.Collections.Generic;
using Expressions.Lexing.AbstractTokenParsers;
using Expressions.Lexing.Tokens;

namespace Expressions.Lexing.TokenParsers
{
    public class NumberParser : SequentialParser
    {
        protected override IEnumerable<ITokenParser> Parsers => new List<ITokenParser>
        {
            new OptionalParser(SignParser),
            new AlternativeParser(
                new UnsignedRealParser(), UnsignedIntegerParser
            )
        };

        protected override Token ResultsToToken(List<Token> results)
        {
            var signToken = OptionalParser.ConvertOrNull<SignToken>(results[0]);
            var number = (UnsignedNumberToken) results[1];

            // Convert real to integer if no fraction and scale are present
            number = number switch
            {
                UnsignedRealNumber r
                    when r.FractionToken == null
                         && r.ScaleFactorToken == null => new UnsignedDecimalInteger(r.IntegerToken),
                _ => number
            };
            
            return new NumberToken(signToken, number);
        }


        public static readonly AlternativeParser SignParser = new(
            new SingleCharacterParser('+', (_, pos) => new PlusSignToken(pos)),
            new SingleCharacterParser('-', (_, pos) => new MinusSignToken(pos))
        );

        public class UnsignedRealParser : SequentialParser
        {
            protected override IEnumerable<ITokenParser> Parsers => new List<ITokenParser>
            {
                new DecimalDigitSequenceParser(),
                new OptionalParser(new FractionParser()),
                new OptionalParser(new ScaleFactorParser())
            };

            protected override Token ResultsToToken(List<Token> results)
            {
                var integerPart = (DecimalDigitSequence) results[0];
                var fractionPart = OptionalParser.ConvertOrNull<FractionToken>(results[1]);
                var scaleFactor = OptionalParser.ConvertOrNull<ScaleFactorToken>(results[2]);
                return new UnsignedRealNumber(integerPart, fractionPart, scaleFactor);
            }
        }

        public class FractionParser : SequentialParser
        {
            protected override IEnumerable<ITokenParser> Parsers => new List<ITokenParser>
            {
                new SingleCharacterParser('.', (_, pos) => new DotToken(pos)),
                new DecimalDigitSequenceParser()
            };

            protected override Token ResultsToToken(List<Token> results) =>
                new FractionToken((DotToken) results[0], (DecimalDigitSequence) results[1]);
        }

        public class ScaleFactorParser : SequentialParser
        {
            protected override IEnumerable<ITokenParser> Parsers => new List<ITokenParser>
            {
                new AlternativeParser(
                    new SingleCharacterParser('e', (t, pos) => new ScaleToken(t, pos)),
                    new SingleCharacterParser('E', (t, pos) => new ScaleToken(t, pos))
                ),
                new OptionalParser(SignParser),
                new DecimalDigitSequenceParser()
            };

            protected override Token ResultsToToken(List<Token> results)
            {
                var scaleChar = (ScaleToken) results[0];
                var signToken = OptionalParser.ConvertOrNull<SignToken>(results[1]);
                var scaleValue = (DecimalDigitSequence) results[2];
                return new ScaleFactorToken(scaleChar, signToken, scaleValue);
            }
        }

        public static readonly AlternativeParser UnsignedIntegerParser = new(
            new BinaryIntegerParser(), new OctalIntegerParser(),
            new DecimalIntegerParser(), new HexIntegerParser()
        );

        public class BinaryIntegerParser : SequentialParser
        {
            protected override IEnumerable<ITokenParser> Parsers => new List<ITokenParser>
            {
                new SingleCharacterParser('%', (_, pos) => new PercentToken(pos)),
                new BinaryDigitSequenceParser()
            };

            protected override Token ResultsToToken(List<Token> results) =>
                new UnsignedBinaryInteger((PercentToken) results[0], (BinaryDigitSequence) results[1]);
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
            {
                new SingleCharacterParser('&', (_, pos) => new AmpersandToken(pos)),
                new OctalDigitSequenceParser()
            };

            protected override Token ResultsToToken(List<Token> results) =>
                new UnsignedOctalInteger((AmpersandToken) results[0], (OctalDigitSequence) results[1]);
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
            {
                new SingleCharacterParser('$', (_, pos) => new DollarSignToken(pos)),
                new HexDigitSequenceParser()
            };

            protected override Token ResultsToToken(List<Token> results) =>
                new UnsignedHexInteger((DollarSignToken) results[0], (HexDigitSequence) results[1]);
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
}