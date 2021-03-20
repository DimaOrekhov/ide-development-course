namespace Expressions
{
    public abstract record Token
    {
        protected Token(string value)
        {
            Value = value;
        }
        
        public readonly string Value;
    }
    
    public record OperatorToken : Token
    {
        public OperatorToken(string value) : base(value) {}
    }

    public record VariableToken : Token
    {
        public VariableToken(string value) : base(value) {}
    }

    public record LiteralToken : Token
    {
        public LiteralToken(string value) : base(value) {}
    }

    public abstract record ParenToken : Token
    {
        protected ParenToken(string value) : base(value) {}
    }

    public record OpeningParenToken : ParenToken
    {
        public OpeningParenToken() : base("(") {}
    }

    public record ClosingParenToken : ParenToken
    {
        public ClosingParenToken() : base(")") {}
    }
}