using System.Collections.Generic;

namespace Expressions.Lexing.Tokens
{
    public record CharacterStringToken : CompoundToken
    {
        public readonly List<CharacterStringElementToken> Elements;
        
        public CharacterStringToken(List<CharacterStringElementToken> elements) : base(elements[0].Start, elements[^1].End)
        {
            Elements = elements;
        }
    }

    public abstract record CharacterStringElementToken : CompoundToken
    {
        protected CharacterStringElementToken(Position start, Position end) : base(start, end)
        {
        }
    }
    
    public record QuotedStringToken : CharacterStringElementToken
    {
        public QuotedStringToken(Position start, Position end) : base(start, end)
        {
        }
    }
    
    public record SingleQuoteToken : ElementaryToken
    {
        public SingleQuoteToken(Position start) : base("'", start, start)
        {
        }
    }

    public record StringBodyToken : CompoundToken
    {
        public StringBodyToken(Position start, Position end) : base(start, end)
        {
        }
    }

    public record ControlStringToken : CharacterStringElementToken
    {
        public readonly UnsignedIntegerToken Value;
        
        public ControlStringToken(HashToken hashToken, UnsignedIntegerToken value) : base(hashToken.Start, value.End)
        {
            Value = value;
        }
    }

    public record HashToken : ElementaryToken
    {
        public HashToken(Position start) : base("#", start, start)
        {
        }
    }
}