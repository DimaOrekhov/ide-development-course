using System.Collections.Generic;
using Expressions.Lexing.AbstractTokenParsers;
using Expressions.Lexing.TokenParsers;
using Expressions.Lexing.Tokens;

namespace Expressions.Lexing
{ 
    public static class Lexer
    {
        private static readonly ITokenParser StepParser = new AlternativeParser(
            new NumberParser(), 
            new IdentifierParser(), 
            new GeneralCommentParser(), 
            CharacterStringParser.Instance, 
            new SpecialSymbolParser());

        private static readonly WhiteSpaceParser WhiteSpaceParser = new WhiteSpaceParser();
        private static readonly Position StartPosition = new Position(0, 0, 0);
        
        public static List<Token> Lex(string text, int absoluteOffset)
        {
            var initialPosition = LexingUtils.UpdatePosition(text, StartPosition, absoluteOffset);
            return Lex(text, initialPosition);
        }
        
        public static List<Token> Lex(string text, Position initialPosition)
        {
            var tokens = new List<Token>();
            var currentPosition = initialPosition;
            while (currentPosition.AbsoluteOffset < text.Length)
            {
                // Skip whitespace if needed
                var whiteSpaceResult = WhiteSpaceParser.Parse(text, currentPosition);
                switch (whiteSpaceResult)
                {
                    case FailedParsingResult f: break;
                    case SuccessfulParsingResult s:
                        var whiteSpaceToken = s.Token;
                        tokens.Add(whiteSpaceToken);
                        var currentAbsoluteOffset = whiteSpaceToken.End.AbsoluteOffset + 1;
                        if (currentAbsoluteOffset >= text.Length)
                        {
                            return tokens;
                        }
                        
                        currentPosition = LexingUtils.UpdatePosition(text, currentPosition, currentAbsoluteOffset);
                        break;
                }

                var result = StepParser.Parse(text, currentPosition);
                if (result is FailedParsingResult)
                {
                    return tokens;
                }

                var token = ((SuccessfulParsingResult) result).Token;
                tokens.Add(token);

                currentPosition = LexingUtils.UpdatePosition(text, currentPosition, token.End.AbsoluteOffset + 1);
            }

            return tokens;
        }
    }
}