namespace Expressions.Lexing.AbstractTokenParsers
{
    public abstract class DfaTokenParser : ITokenParser
    {
        public abstract class DfaState
        {
            public readonly bool IsTerminal;

            protected DfaState(bool isTerminal)
            {
                IsTerminal = isTerminal;
            }
        }

        public class TerminalState : DfaState
        {
            public TerminalState() : base(true)
            {
            }
        }

        public class NonTerminalState : DfaState
        {
            public NonTerminalState() : base(false)
            {
            }
        }

        public class DeadState : NonTerminalState
        {
            public static readonly DeadState Instance = new();
        }

        public class GeneralInitialState : NonTerminalState
        {
            public static readonly GeneralInitialState Instance = new();
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
                var currentPosition = LexingUtils.UpdatePosition(text, lastPosition, i);
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

            // Maybe start with lastPosition.AbsoluteOffset + 1 and look at symbol i - 1?
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

    public abstract class TableDfaTokenParser : DfaTokenParser
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