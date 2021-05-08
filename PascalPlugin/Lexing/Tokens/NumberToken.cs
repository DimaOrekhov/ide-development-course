namespace Expressions.Lexing.Tokens
{
    public record NumberToken : CompoundToken
    {
        public readonly SignToken Sign;
        public readonly UnsignedNumberToken Number;

        public NumberToken(SignToken sign, UnsignedNumberToken numberToken) 
            : base(sign?.Start ?? numberToken.Start, numberToken.End)
        {
            Sign = sign;
            Number = numberToken;
        }
    }
    
    public abstract record SignToken : SpecialSymbolToken
    {
        protected SignToken(string value, Position start) : base(value, start, start)
        {
        }
    }

    public record PlusSignToken : SignToken
    {
        public PlusSignToken(Position start) : base("+", start)
        {
        }
    }

    public record MinusSignToken : SignToken
    {
        public MinusSignToken(Position start) : base("-", start)
        {
        }
    }

    public abstract record UnsignedNumberToken : CompoundToken
    {
        protected UnsignedNumberToken(Position start, Position end) : base(start, end)
        {
        }
    }

    public record BinaryDigitSequence : ElementaryToken
    {
        public BinaryDigitSequence(string value, Position start, Position end) : base(value, start, end)
        {
        }
    }

    public record OctalDigitSequence : ElementaryToken
    {
        public OctalDigitSequence(string value, Position start, Position end) : base(value, start, end)
        {
        }
    }
    
    public record DecimalDigitSequence : ElementaryToken
    {
        public DecimalDigitSequence(string value, Position start, Position end) : base(value, start, end)
        {
        }
    }

    public record HexDigitSequence : ElementaryToken
    {
        public HexDigitSequence(string value, Position start, Position end) : base(value, start, end)
        {
        }
    }
    
    public record UnsignedRealNumber : UnsignedNumberToken
    {
        public readonly DecimalDigitSequence IntegerToken;
        public readonly FractionToken FractionToken;
        public readonly ScaleFactorToken ScaleFactorToken;
        
        public UnsignedRealNumber(DecimalDigitSequence integerToken, FractionToken fractionToken = null, ScaleFactorToken scaleFactorToken = null)
            : base(integerToken.Start, 
                scaleFactorToken?.End ?? fractionToken?.End ?? integerToken.End)
        {
            IntegerToken = integerToken;
            FractionToken = fractionToken;
            ScaleFactorToken = scaleFactorToken;
        }
    }

    public record FractionToken : CompoundToken
    {
        public readonly DotToken DotToken;
        public readonly DecimalDigitSequence DigitSequence;
        
        public FractionToken(DotToken dotToken, DecimalDigitSequence digitSequence) 
            : base(dotToken.Start, digitSequence.End)
        {
            DotToken = dotToken;
            DigitSequence = digitSequence;
        }
    }
    
    public record DotToken : ElementaryToken
    {
        public DotToken(Position start) : base(".", start, start)
        {
        }
    }

    public record ScaleFactorToken : CompoundToken
    {
        public readonly ScaleToken ScaleToken;
        public readonly SignToken Sign;
        public readonly DecimalDigitSequence ScaleValue;
        
        public ScaleFactorToken(ScaleToken scaleToken, SignToken sign, DecimalDigitSequence scaleValue) 
            : base(scaleToken.Start, scaleValue.End)
        {
            ScaleToken = scaleToken;
            Sign = sign;
            ScaleValue = scaleValue;
        }
    }

    public record ScaleToken : ElementaryToken
    {
        public ScaleToken(string value, Position start) : base(value, start, start)
        {
        }
    }

    public abstract record UnsignedIntegerToken : UnsignedNumberToken
    {
        protected UnsignedIntegerToken(Position start, Position end) : base(start, end)
        {
        }
    }

    public record UnsignedDecimalInteger : UnsignedIntegerToken
    {
        public readonly DecimalDigitSequence DigitSequence;
        
        public UnsignedDecimalInteger(DecimalDigitSequence digitSequence) : base(digitSequence.Start, digitSequence.End)
        {
            DigitSequence = digitSequence;
        }
    }
    
    public record UnsignedBinaryInteger : UnsignedIntegerToken
    {
        public readonly PercentToken PercentTokenToken;
        public readonly BinaryDigitSequence DigitSequence;
        
        public UnsignedBinaryInteger(PercentToken percentToken, BinaryDigitSequence digitSequence) 
            : base(percentToken.Start, digitSequence.End)
        {
            PercentTokenToken = percentToken;
            DigitSequence = digitSequence;
        }
    }

    public record PercentToken : ElementaryToken
    {
        public PercentToken(Position start) : base("%", start, start)
        {
        }
    }
    
    public record UnsignedOctalInteger : UnsignedIntegerToken
    {
        public readonly AmpersandToken AmpersandToken;
        public readonly OctalDigitSequence DigitSequence;
        
        public UnsignedOctalInteger(AmpersandToken ampersandToken, OctalDigitSequence digitSequence) 
            : base(ampersandToken.Start, digitSequence.End)
        {
            AmpersandToken = ampersandToken;
            DigitSequence = digitSequence;
        }
    }

    public record AmpersandToken : ElementaryToken
    {
        public AmpersandToken(Position start) : base("&", start, start)
        {
        }
    }
    
    public record UnsignedHexInteger : UnsignedIntegerToken
    {
        public readonly DollarSignToken DollarSignToken;
        public readonly HexDigitSequence DigitSequence;
        
        public UnsignedHexInteger(DollarSignToken dollarSignToken, HexDigitSequence digitSequence) 
            : base(dollarSignToken.Start, digitSequence.End)
        {
            DollarSignToken = dollarSignToken;
            DigitSequence = digitSequence;
        }
    }

    public record DollarSignToken : ElementaryToken
    {
        public DollarSignToken(Position start) : base("$", start, start)
        {
        }
    }
}
