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
            public abstract bool IsTerminal { get; }
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

        protected abstract DfaState InitialState { get; }

        protected abstract DfaState Transition(DfaState currentState, string text, ref int currentAbsoluteOffset);

        protected abstract ParsingResult GetCurrentResult(string text, Position start, Position end);

        public ParsingResult Parse(string text, Position initialPosition)
        {
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

                if (!currentState.IsTerminal)
                {
                    continue;
                }

                var lastPosition = GetPosition(result, initialPosition);
                var currentPosition = GetCurrentPositions(text, lastPosition, i);
                result = GetCurrentResult(text, initialPosition, currentPosition);

                i++;
            }

            result ??= new FailedParsingResult(); // TODO: Maybe add some info, e.g. up to where you could parse
            return result;
        }

        private static Position GetPosition(ParsingResult result, Position defaultPosition) =>
            result switch
            {
                null => defaultPosition,
                FailedParsingResult => defaultPosition,
                SuccessfulParsingResult s => s.Token.End
            };

        private static Position GetCurrentPositions(string text, Position lastPosition, int currentAbsoluteOffset)
        {
            var currentLine = lastPosition.Line;
            var currentOffset = lastPosition.Offset;

            // Kind of strange behaviour if token starts/ends with newline character,
            // but I suppose there won't be such tokens
            for (var i = lastPosition.AbsoluteOffset; i < currentAbsoluteOffset; i++)
            {
                if (text[i] == '\n')
                {
                    currentLine++;
                    currentOffset = 0;
                }
                else
                {
                    currentOffset++;
                }
            }

            return new Position(currentLine, currentOffset, currentAbsoluteOffset);
        }
    }

    public abstract class TableDfaParser : DfaParser
    {
        protected abstract DfaState[,] TransitionTable { get; }

        protected abstract int GetStateIndex(DfaState state);

        protected abstract int
            GetSymbolIndex(char symbol); // TODO: maybee rename to get seq index and change signature to string + int

        protected override DfaState Transition(DfaState currentState, string text, ref int currentAbsoluteOffset)
        {
            var nextState = TransitionTable[GetStateIndex(currentState), GetSymbolIndex(text[currentAbsoluteOffset])];
            return nextState;
        }
    }
}