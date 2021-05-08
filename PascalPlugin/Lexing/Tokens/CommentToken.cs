using System.Collections.Generic;

namespace Expressions.Lexing.Tokens
{
    public abstract record CommentToken : CompoundToken
    {
        public readonly List<CommentToken> NestedComments;
        
        public CommentToken(List<CommentToken> nestedComments, Position start, Position end) : base(start, end)
        {
            NestedComments = nestedComments;
        }
    }

    public record LineComment : CommentToken
    {
        public LineComment(List<CommentToken> nestedComments, Position start, Position end) : base(nestedComments, start, end)
        {
        }
    }

    public record RoundBracketComment : CommentToken
    {
        public RoundBracketComment(List<CommentToken> nestedComments, Position start, Position end) : base(nestedComments, start, end)
        {
        }
    }

    public record CurlyBracketComment : CommentToken
    {
        public CurlyBracketComment(List<CommentToken> nestedComments, Position start, Position end) : base(nestedComments, start, end)
        {
        }
    }
}