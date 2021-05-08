using Expressions.Lexing;
using Expressions.Lexing.AbstractTokenParsers;
using Expressions.Lexing.TokenParsers;
using NUnit.Framework;

namespace ExpressionsTests.Lexing.TokenParsers
{
    public class NumberParserTests
    {
        private static readonly NumberParser NumberParser = new();
        private static readonly ITokenParser UnsignedIntegerParser = NumberParser.UnsignedIntegerParser;
        private static readonly NumberParser.UnsignedRealParser UnsignedRealParser = new();
        
        [Test]
        public void TestUnsignedIntegerParser()
        {
            var parser = UnsignedIntegerParser;
            parser.ParseToken<UnsignedBinaryInteger>("%01010", Utils.InitialPosition);
            parser.ParseToken<UnsignedOctalInteger>("&07020", Utils.InitialPosition);
            parser.ParseToken<UnsignedHexInteger>("$AF112", Utils.InitialPosition);
            parser.ParseToken<UnsignedDecimalInteger>("12304", Utils.InitialPosition);
        }

        private static void ParseAndAssertUnsignedReal(string text, string integerPart, string fractionPart = null, 
            string scale = null)
        {
            var parser = UnsignedRealParser;
            var token = parser.ParseToken<UnsignedRealNumber>(text);
            
            AssertUnsignedRealToken(token, integerPart, fractionPart, scale);
        }

        private static void AssertUnsignedRealToken(UnsignedRealNumber token, string integerPart,
            string fractionPart = null,
            string scale = null)
        {
            Assert.AreEqual(integerPart, token.IntegerToken.Value);
            
            Assert.AreEqual(fractionPart, token.FractionToken?.DigitSequence?.Value);

            if (scale != null)
            {
                Assert.AreEqual(scale,
                    (token.ScaleFactorToken.Sign?.Value ?? "") + token.ScaleFactorToken.ScaleValue.Value);
            }
            else
            {
                Assert.AreEqual(scale, token.ScaleFactorToken);
            }
        }

        [Test]
        public void TestUnsignedRealParser()
        {
            ParseAndAssertUnsignedReal("10.12", "10", "12");
            ParseAndAssertUnsignedReal("0.1", "0", "1");
            ParseAndAssertUnsignedReal("185e90", "185", scale: "90");
            ParseAndAssertUnsignedReal("10.11e10", "10", "11", "10");
            ParseAndAssertUnsignedReal("10.11e-10", "10", "11", "-10");
        }

        private static void ParseAndAssertSignedReal(string text, string integerPart, string sign = null, string fractionPart = null,
            string scale = null)
        {
            var parser = NumberParser;
            var token = parser.ParseToken<NumberToken>(text);

            if (sign != null)
            {
                Assert.AreEqual(sign, token.Sign.Value);
            }
            
            Assert.IsInstanceOf<UnsignedRealNumber>(token.Number);
            var numberToken = (UnsignedRealNumber) token.Number;
            AssertUnsignedRealToken(numberToken, integerPart, fractionPart, scale);
        }

        [Test]
        public void TestSignedRealParsing()
        {
            ParseAndAssertSignedReal("+10.1", "10", "+", "1");
            ParseAndAssertSignedReal("-5.0", "5", "-", "0");
            ParseAndAssertSignedReal("10.1", "10", fractionPart: "1");
            ParseAndAssertSignedReal("-80e-20", "80", "-", scale: "-20");
        }

        private static void ParseAndAssertSignedInteger<T>(string text, string sign = null)
            where T : UnsignedIntegerToken
        {
            var token = NumberParser.ParseToken<NumberToken>(text);
            Assert.AreEqual(sign, token.Sign?.Value);
            
            Assert.IsInstanceOf<T>(token.Number);
        }
        
        [Test]
        public void TestSignedIntegerParsing()
        {
            ParseAndAssertSignedInteger<UnsignedBinaryInteger>("%01010");
            ParseAndAssertSignedInteger<UnsignedOctalInteger>("&07020");
            ParseAndAssertSignedInteger<UnsignedHexInteger>("$AF112");
            ParseAndAssertSignedInteger<UnsignedDecimalInteger>("12304");
            
            ParseAndAssertSignedInteger<UnsignedBinaryInteger>("+%01010", "+");
            ParseAndAssertSignedInteger<UnsignedOctalInteger>("+&07020", "+");
            ParseAndAssertSignedInteger<UnsignedHexInteger>("+$AF112", "+");
            ParseAndAssertSignedInteger<UnsignedDecimalInteger>("+12304", "+");
            
            ParseAndAssertSignedInteger<UnsignedBinaryInteger>("-%01010", "-");
            ParseAndAssertSignedInteger<UnsignedOctalInteger>("-&07020", "-");
            ParseAndAssertSignedInteger<UnsignedHexInteger>("-$AF112", "-");
            ParseAndAssertSignedInteger<UnsignedDecimalInteger>("-12304", "-");
        }
    }
}