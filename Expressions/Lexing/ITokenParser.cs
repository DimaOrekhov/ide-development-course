using System;
using System.Collections.Generic;

namespace Expressions.Lexing
{
    public interface ITokenParser
    {
        ParsingResult Parse(string text, Position initialPosition);
    }

    public class ParsingResult
    {
    }

    public class SuccessfulParsingResult : ParsingResult
    {
        public readonly Token Token;

        public SuccessfulParsingResult(Token token)
        {
            Token = token;
        }
    }

    public class FailedParsingResult : ParsingResult
    {
    }


    public abstract class DfaParser : ITokenParser
    {
        public abstract class DfaState
        {
            public abstract bool IsTerminal
            {
                get;
            }
        }

        public class TerminalState : DfaState
        {
            public override bool IsTerminal => true;
        }

        public class NonTerminalState : DfaState
        {
            public override bool IsTerminal => false;
        }

        public class DeadState : NonTerminalState
        {
            public static readonly DeadState Instance = new();
        }

        protected abstract DfaState InitialState
        {
            get;
        }
        
        protected abstract DfaState Transition(DfaState currentState, string text, ref int currentAbsoluteOffset);

        protected abstract ParsingResult GetCurrentResult(string text, Position start, Position end);

        public ParsingResult Parse(string text, Position initialPosition)
        {
            var passedLines = 0;
            var currentOffset = initialPosition.Offset;
            
            var currentState = InitialState;
            var result = currentState.IsTerminal ? GetCurrentResult(text, initialPosition, initialPosition) : null;

            var i = initialPosition.AbsoluteOffset;
            while (i < text.Length)
            {
                currentState = Transition(currentState, text, ref i);

                // Break if nowhere to go:
                if (currentState is DeadState)
                {
                    break;
                }
                
                // Update position
                if (text[i] == '\n')
                {
                    passedLines++;
                    currentOffset = 0;
                }
                else
                {
                    currentOffset++;
                }

                if (!currentState.IsTerminal)
                {
                    continue;
                }
                
                var currentPosition = new Position(line: initialPosition.Line + passedLines, offset: currentOffset, 
                    absoluteOffset: i + 1); // TODO: Not sure about absoluteOffset
                result = GetCurrentResult(text, initialPosition, currentPosition);
            }

            result ??= new FailedParsingResult(); // TODO: Maybe add some info
            return result;
        }
    }
    
    public abstract class TableDfaParser : DfaParser
    {
        protected abstract DfaState[,] TransitionTable
        {
            get;
        }

        protected abstract int GetStateIndex(DfaState state);

        protected abstract int GetSymbolIndex(char symbol); // TODO: maybee rename to get seq index and change signature to string + int

        protected override DfaState Transition(DfaState currentState, string text, ref int currentAbsoluteOffset)
        {
            var nextState = TransitionTable[GetStateIndex(currentState), GetSymbolIndex(text[currentAbsoluteOffset])];
            
            if (!(nextState is DeadState))
            {
                currentAbsoluteOffset++;
            }
            
            return nextState;
        }
    }
}