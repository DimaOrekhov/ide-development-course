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

        // TODO:
        // Maybe some symbols should be excluded since they are part of other tokens:
        // e.g. ' for char string, # for CR
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
                Initial => ParseSpecialSymbol(text, ref currentAbsoluteOffset),
                TerminalState => DeadState.Instance
            };

        private static DfaState ParseSpecialSymbol(string text, ref int currentAbsoluteOffset)
        {
            var numberOfParsedSymbols = 0;
            var start = currentAbsoluteOffset;
            if (TwoSymbolSpecials.FirstOrDefault(t => text.AsSpan(start, 2).SequenceEqual(t)) != default)
            {
                numberOfParsedSymbols = 2;
            }
            else if (SingleSymbolSpecials.FirstOrDefault(t => text[start] == t) != default)
            {
                numberOfParsedSymbols = 1;
            }

            currentAbsoluteOffset += numberOfParsedSymbols;
            return numberOfParsedSymbols != 0 ? Terminal : DeadState.Instance;
        }

        protected override ParsingResult GetCurrentResult(string text, Position start, Position end)
        {
            var value = text.Substring(start.AbsoluteOffset, end.AbsoluteOffset - start.AbsoluteOffset + 1);
            throw new System.NotImplementedException();
        }
    }
}