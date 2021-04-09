using System;

namespace Expressions.Lexing.TokenParsers
{
    public class IdentifierParser : TableDfaParser
    {
        // States:
        private class IdentifierInitial : NonTerminalState
        {
            public static readonly IdentifierInitial Instance = new();
        }

        private class FirstSymbolParsed : TerminalState
        {
            public static readonly FirstSymbolParsed Instance = new();
        }

        private class ManySymbolsParsed : TerminalState
        {
            public static readonly ManySymbolsParsed Instance = new();
        }
        
        // Transition logic:
        protected override DfaState InitialState => IdentifierInitial.Instance;

        private static readonly DfaState[,] Table =
        {
            {FirstSymbolParsed.Instance, FirstSymbolParsed.Instance, DeadState.Instance, DeadState.Instance}, // Initial row
            {ManySymbolsParsed.Instance, ManySymbolsParsed.Instance, ManySymbolsParsed.Instance, DeadState.Instance}, // FirstSymbolParsedRow
            {ManySymbolsParsed.Instance, ManySymbolsParsed.Instance, ManySymbolsParsed.Instance, DeadState.Instance}  // Continuation
        };
        protected override DfaState[,] TransitionTable => Table;
        
        protected override int GetStateIndex(DfaState state) => state switch
        {
            IdentifierInitial => 0,
            FirstSymbolParsed => 1,
            ManySymbolsParsed => 2
        };

        protected override int GetSymbolIndex(char symbol) => symbol switch
        {
            var c when char.IsLetter(c) => 0,
            var c when c == '_' => 1,
            var c when char.IsDigit(c) => 2,
            _ => 3
        };

        protected override ParsingResult GetCurrentResult(string text, Position start, Position end)
        {
            var value = text.Substring(start.AbsoluteOffset, end.AbsoluteOffset - start.AbsoluteOffset);
            var token = new IdentifierToken(value, start, end);
            return new SuccessfulParsingResult(token);
        }
    }
}