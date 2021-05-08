using System;
using System.Collections.Generic;
using Expressions.Lexing.AbstractTokenParsers;
using Expressions.Lexing.Tokens;

namespace Expressions.Lexing.TokenParsers
{
    public class GeneralCommentParser : ITokenParser
    {
        public GeneralCommentParser()
        {
        }
        
        public ParsingResult Parse(string text, Position initialPosition)
        {
            var topLevelParser = ChooseCommentParser(text, initialPosition.AbsoluteOffset, null);
            return topLevelParser == null ? new FailedParsingResult() : topLevelParser.Parse(text, initialPosition);
        }

        private static CommentParser ChooseCommentParser(string text, int position, CommentParser parent)
        {
            var textSpan = text.AsSpan(position);
            
            if (textSpan.StartsWith(Constants.SingleLineOpening))
            {
                return new SingleLineCommentParser(parent);
            }

            if (textSpan.StartsWith(Constants.CurlyOpening))
            {
                return new CurlyBracketsCommentParser(parent);
            }

            if (textSpan.StartsWith(Constants.RoundOpening))
            {
                return new RoundBracketCommentParser(parent);
            }
            
            return null;
        }
        
        private static class Constants
        {
            public const string SingleLineOpening = "//";
            public const string SingleLineClosing = "\n";
            public const string CurlyOpening = "{";
            public const string CurlyClosing = "}";
            public const string RoundOpening = "(*";
            public const string RoundClosing = "*)";
        }

        private abstract class CommentParser : ITokenParser
        {
            protected readonly CommentParser ParentParser;

            protected CommentParser(CommentParser parentParser = null)
            {
                ParentParser = parentParser;
            }
            
            public abstract string OpeningSymbols
            {
                get;
            }

            public abstract string ClosingSymbols
            {
                get;
            }
            public abstract ParsingResult Parse(string text, Position initialPosition);

            protected abstract CommentToken CreateToken(string text, Position initialPosition,
                int currentAbsoluteOffset, List<CommentToken> nestedComments);
        }

        private class SingleLineCommentParser : CommentParser
        {
            public SingleLineCommentParser(CommentParser parentParser) : base(parentParser)
            {
            }
            
            protected override LineComment CreateToken(string text, Position initialPosition, int currentAbsoluteOffset, List<CommentToken> nestedComments)
            {
                var currentPosition = LexingUtils.UpdatePosition(text, initialPosition, currentAbsoluteOffset);
                return new LineComment(nestedComments, initialPosition, currentPosition);
            }

            public override string OpeningSymbols => Constants.SingleLineOpening;
            public override string ClosingSymbols => Constants.SingleLineClosing;

            public override ParsingResult Parse(string text, Position initialPosition)
            {
                var nestedComments = new List<CommentToken>();
                
                for (var i = initialPosition.AbsoluteOffset + OpeningSymbols.Length; i < text.Length;)
                {
                    var nestedParser = ChooseCommentParser(text, i, ParentParser ?? this);
                    if (nestedParser != null)
                    {
                        // For single line comment nested comments are only possible if parent is null
                        // Otherwise let's consider it a nested comment of its parent
                        if (ParentParser != null)
                        {
                            return new SuccessfulParsingResult(CreateToken(text, initialPosition, i - 1, nestedComments));
                        }
                        
                        var currentPosition = LexingUtils.UpdatePosition(text, initialPosition, i);
                        switch (nestedParser.Parse(text, currentPosition))
                        {
                            case SuccessfulParsingResult s:
                                nestedComments.Add((CommentToken) s.Token);
                                i = s.Token.End.AbsoluteOffset + 1;
                                continue; // continue cycle
                            case FailedParsingResult f:
                                return f;
                        }
                    }

                    // Parent comment is ended
                    if (ParentParser != null
                        && text.AsSpan(i).StartsWith(ParentParser.ClosingSymbols))
                    {
                        var token = CreateToken(text, initialPosition, i - 1, nestedComments);
                        return new SuccessfulParsingResult(token);
                    }
                    
                    // Single line comment is ended
                    if (text.AsSpan(i).StartsWith(ClosingSymbols))
                    {
                        var token = CreateToken(text, initialPosition, i + ClosingSymbols.Length - 1, nestedComments);
                        return new SuccessfulParsingResult(token);
                    }

                    i++;
                }
                
                // EOF is a correct termination of a line comment
                return new SuccessfulParsingResult(CreateToken(text, initialPosition, text.Length - 1, nestedComments));
            }
        }
        
        private abstract class MultiLineCommentParser : CommentParser
        {
            protected MultiLineCommentParser(CommentParser parentParser) : base(parentParser)
            {
            }
            
            public override ParsingResult Parse(string text, Position initialPosition)
            {
                var nestedComments = new List<CommentToken>();

                for (var i = initialPosition.AbsoluteOffset + OpeningSymbols.Length; i < text.Length;)
                {
                    var nestedParser = ChooseCommentParser(text, i, this);
                    if (nestedParser != null)
                    {
                        var currentPosition = LexingUtils.UpdatePosition(text, initialPosition, i);
                        switch (nestedParser.Parse(text, currentPosition))
                        {
                            case SuccessfulParsingResult s:
                                nestedComments.Add((CommentToken) s.Token);
                                i = s.Token.End.AbsoluteOffset + 1;
                                continue; // continue cycle
                            case FailedParsingResult f:
                                return f;
                        }
                    }
                    
                    // Closing itself:
                    if (text.AsSpan(i).StartsWith(ClosingSymbols))
                    {
                        var token = CreateToken(text, initialPosition, i + ClosingSymbols.Length - 1, nestedComments);
                        return new SuccessfulParsingResult(token);
                    }

                    if (ParentParser != null && text.AsSpan(i).StartsWith(ParentParser.ClosingSymbols))
                    {
                        return new FailedParsingResult();
                    }
                    
                    i++;
                }

                return new FailedParsingResult();
            }
        }

        private class RoundBracketCommentParser : MultiLineCommentParser
        {
            public RoundBracketCommentParser(CommentParser parentParser) : base(parentParser)
            {
            }

            public override string OpeningSymbols => Constants.RoundOpening;
            public override string ClosingSymbols => Constants.RoundClosing;
            
            protected override RoundBracketComment CreateToken(string text, Position initialPosition, int currentAbsoluteOffset, List<CommentToken> nestedComments)
            {
                var currentPosition = LexingUtils.UpdatePosition(text, initialPosition, currentAbsoluteOffset);
                return new RoundBracketComment(nestedComments, initialPosition, currentPosition);            
            }
        }

        private class CurlyBracketsCommentParser : MultiLineCommentParser
        {
            public CurlyBracketsCommentParser(CommentParser parentParser) : base(parentParser)
            {
            }
            
            public override string OpeningSymbols => Constants.CurlyOpening;
            public override string ClosingSymbols => Constants.CurlyClosing;
            
            protected override CurlyBracketComment CreateToken(string text, Position initialPosition, int currentAbsoluteOffset, List<CommentToken> nestedComments)
            {
                var currentPosition = LexingUtils.UpdatePosition(text, initialPosition, currentAbsoluteOffset);
                return new CurlyBracketComment(nestedComments, initialPosition, currentPosition);
            }
        }
    }
}
