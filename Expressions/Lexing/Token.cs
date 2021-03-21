namespace Expressions.Lexing
{
    public abstract record Token
    {
        protected Token(string value, int position)
        {
            Value = value;
            Position = position;
        }
        
        public readonly string Value;
        public readonly int Position;
    }
    
    public record OperatorToken : Token
    {
        public OperatorToken(string value, int position) : base(value, position) {}
    }

    public record VariableToken : Token
    {
        public VariableToken(string value, int position) : base(value, position) {}
    }

    public record LiteralToken : Token
    {
        public LiteralToken(string value, int position) : base(value, position) {}
    }

    public abstract record ParenToken : Token
    {
        protected ParenToken(string value, int position) : base(value, position) {}
    }

    public record OpeningParenToken : ParenToken
    {
        public OpeningParenToken(int position) : base("(", position) {}
    }

    public record ClosingParenToken : ParenToken
    {
        public ClosingParenToken(int position) : base(")", position) {}
    }
}