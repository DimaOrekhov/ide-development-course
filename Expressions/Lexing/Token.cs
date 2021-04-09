namespace Expressions.Lexing
{
    public abstract record Token
    {
        protected Token(string value, Position start, Position end)
        {
            Value = value;
            Start = start;
            End = end;
        }
        
        public readonly string Value;
        public readonly Position Start;
        public readonly Position End;
    }

    public record Position
    {
        public Position(int line, int offset, int absoluteOffset)
        {
            Line = line;
            Offset = offset;
            AbsoluteOffset = absoluteOffset;
        }

        public readonly int Line;
        public readonly int Offset;
        public readonly int AbsoluteOffset;
    }
    
    public record OperatorToken : Token
    {
        public OperatorToken(string value, Position start, Position end) : base(value, start, end) {}
    }

    public record IdentifierToken : Token
    {
        public IdentifierToken(string value, Position start, Position end) : base(value, start, end) {}
    }

    public record LiteralToken : Token
    {
        public LiteralToken(string value, Position start, Position end) : base(value, start, end) {}
    }

    public abstract record ParenToken : Token
    {
        protected ParenToken(string value, Position start, Position end) : base(value, start, end) {}
    }

    public record OpeningParenToken : ParenToken
    {
        public OpeningParenToken(Position start, Position end) : base("(", start, end) {}
    }

    public record ClosingParenToken : ParenToken
    {
        public ClosingParenToken(Position start, Position end) : base(")", start, end) {}
    }
}