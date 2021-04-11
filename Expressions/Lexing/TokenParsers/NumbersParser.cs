using Expressions.Lexing.AbstractTokenParsers;

namespace Expressions.Lexing.TokenParsers
{
    public record SignedNumberToken : Token
    {
        public SignedNumberToken(string value, Position start, Position end) : base(value, start, end)
        {
            
        }
    }

    public record SignToken : Token
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
}