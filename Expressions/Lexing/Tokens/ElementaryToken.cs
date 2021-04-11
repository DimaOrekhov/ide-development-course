namespace Expressions.Lexing.Tokens
{
    public abstract record Token
    {
        public readonly Position Start;
        public readonly Position End;

        protected Token(Position start, Position end)
        {
            Start = start;
            End = end;
        }
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
    
    public abstract record ElementaryToken : Token
    {
        protected ElementaryToken(string value, Position start, Position end) : base(start, end)
        {
            Value = value;
        }
        
        public readonly string Value;
    }

    public abstract record CompoundToken : Token
    {
        protected CompoundToken(Position start, Position end) : base(start, end)
        {
        }
    }

    public record SpecialSymbolToken : ElementaryToken
    {
        public SpecialSymbolToken(string value, Position start, Position end) : base(value, start, end) {}
    }
    
    public record OperatorToken : ElementaryToken
    {
        public OperatorToken(string value, Position start, Position end) : base(value, start, end) {}
    }

    public record IdentifierToken : ElementaryToken
    {
        public IdentifierToken(string value, Position start, Position end) : base(value, start, end) {}
    }

    public record LiteralToken : ElementaryToken
    {
        public LiteralToken(string value, Position start, Position end) : base(value, start, end) {}
    }

    public abstract record ParenToken : ElementaryToken
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