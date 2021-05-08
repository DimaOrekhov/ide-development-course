using Expressions.Lexing.Tokens;

namespace Expressions.Lexing
{
    public static class LexingUtils
    {
        public static Position UpdatePosition(string text, Position lastPosition, int currentAbsoluteOffset)
        {
            var currentLine = lastPosition.Line;
            var currentOffset = lastPosition.Offset;
            
            for (var i = lastPosition.AbsoluteOffset + 1; i <= currentAbsoluteOffset; i++)
            {
                if (text[i - 1] == '\n')
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

        public static bool IsLatinLetter(this char character) =>
            character >= 'A' && character <= 'Z' 
            || character >= 'a' && character <= 'z';
    }
}