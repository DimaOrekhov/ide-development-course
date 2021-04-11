using System;
using System.Collections.Generic;
using Expressions.Lexing.AbstractTokenParsers;
using Expressions.Lexing.Tokens;

namespace Expressions.Lexing.TokenParsers
{
    public record SignedNumberToken : ElementaryToken
    {
        public SignedNumberToken(string value, Position start, Position end) : base(value, start, end)
        {
            
        }
    }

    public record SignToken : ElementaryToken
    {
        public SignToken(string value, Position start, Position end) : base(value, start, end)
        {
            
        }
    }
    
    public class NumbersParser : TableDfaTokenParser
    {
        // States
        private class Initial : NonTerminalState
        {
            public static readonly Initial Instance = new();
        }

        private static readonly TerminalState Terminal = new();

        protected override DfaState InitialState => Initial.Instance;
        protected override ParsingResult GetCurrentResult(string text, Position start, Position end)
        {
            throw new System.NotImplementedException();
        }

        private static readonly DfaState[,] Table = null; // TODO:
        protected override DfaState[,] TransitionTable { get; }
        protected override int GetStateIndex(DfaState state)
        {
            throw new System.NotImplementedException();
        }

        protected override int GetSymbolIndex(char symbol)
        {
            throw new System.NotImplementedException();
        }
    }

    public class UnsignedIntegerParser : TableDfaTokenParser
    {
        // States:
        
        protected override DfaState InitialState => GeneralInitialState.Instance;
        protected override ParsingResult GetCurrentResult(string text, Position start, Position end)
        {
            throw new System.NotImplementedException();
        }

        protected override DfaState[,] TransitionTable { get; }
        protected override int GetStateIndex(DfaState state)
        {
            throw new System.NotImplementedException();
        }

        protected override int GetSymbolIndex(char symbol)
        {
            throw new System.NotImplementedException();
        }
    }

    public class DigitSequenceParser : PredicateTokenParser
    {
        protected override Predicate<char> Predicate => char.IsDigit;
        
        protected override ElementaryToken MatchedSymbolsToToken(string match, Position start, Position end)
        {
            throw new NotImplementedException();
        }
    }

    public class HexDigitSequenceParser : PredicateTokenParser
    {
        private static bool IsHexDigit(char obj) =>
            char.IsDigit(obj)
            || obj >= 'A' && obj <= 'F'
            || obj >= 'a' && obj <= 'f';
        
        protected override Predicate<char> Predicate => IsHexDigit;

        protected override ElementaryToken MatchedSymbolsToToken(string match, Position start, Position end)
        {
            throw new NotImplementedException();
        }
    }
    
    public class HexIntegerParser : SequentialParser
    {
        protected override IEnumerable<ITokenParser> Parsers => new List<ITokenParser> 
            {new SingleCharacterParser('$'), new HexDigitSequenceParser()};
        
        protected override ElementaryToken ResultsToToken(List<ElementaryToken> results)
        {
            throw new NotImplementedException();
        }
    }
}