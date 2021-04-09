using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressions.Lexing.TokenParsers
{
    public class SpecialSymbolParser : DfaParser
    {
        // States:
        private class Initial : NonTerminalState
        {
            public static readonly Initial Instance = new();
        }

        private static readonly TerminalState Terminal = new();

        private static readonly List<char> SingleSymbolSpecials = new()
        {
            '\'', '+', '-', '*', '/', '=', '<', '>',
            '[', ']', '.', ',', '(', ')', ':', '^', '@',
            '{', '}', '$', '#', '&', '%'
        };

        private static readonly List<string> TwoSymbolSpecials = new()
        {
            "<<", ">>", "**", "<>", "><",
            "<=", ">=", ":=", "+=", "-=", "*=", "/=",
            "(*", "*)", "(.", ".)", "//"
        };

        protected override DfaState InitialState => Initial.Instance;

        protected override DfaState Transition(DfaState currentState, string text, ref int currentAbsoluteOffset) =>
            currentState switch
            {
                Initial => parseSpecialSymbol(text, ref currentAbsoluteOffset),
                TerminalState => DeadState.Instance
            };

        private DfaState parseSpecialSymbol(string text, ref int currentAbsoluteOffset)
        {
            var numberOfParsedSymbols = 0;
            if (TwoSymbolSpecials.FirstOrDefault(text.StartsWith) != default)
            {
                numberOfParsedSymbols = 2;
            }
            else if (SingleSymbolSpecials.FirstOrDefault(text.StartsWith) != default)
            {
                numberOfParsedSymbols = 1;
            }

            currentAbsoluteOffset += numberOfParsedSymbols;
            return numberOfParsedSymbols != 0 ? Terminal : DeadState.Instance;
        }

        protected override ParsingResult GetCurrentResult(string text, Position start, Position end)
        {
            throw new System.NotImplementedException();
        }
    }
}