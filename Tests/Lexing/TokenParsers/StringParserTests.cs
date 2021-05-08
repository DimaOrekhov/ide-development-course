using System;
using System.Linq;
using Expressions.Lexing;
using Expressions.Lexing.AbstractTokenParsers;
using Expressions.Lexing.TokenParsers;
using Expressions.Lexing.Tokens;
using NUnit.Framework;

namespace ExpressionsTests.Lexing.TokenParsers
{
    public class StringParserTests
    {
        private static readonly ITokenParser Parser = CharacterStringParser.Instance;

        private static void ParseAndAssertControlString(string text, string number)
        {
            var token = Parser.ParseToken<CharacterStringToken>(text);
            Assert.AreEqual(1, token.Elements.Count);
            Assert.IsInstanceOf<ControlStringToken>(token.Elements[0]);

            var controlToken = (ControlStringToken) token.Elements[0];
            var actualNumber = ((UnsignedDecimalInteger) controlToken.Value).DigitSequence.Value;
            
            Assert.AreEqual(number, actualNumber);
            
            Assert.AreEqual(Utils.InitialPosition, controlToken.Start);
            Assert.AreEqual(LexingUtils.UpdatePosition(text, Utils.InitialPosition, text.Length - 1), controlToken.End);
        }
        
        [Test]
        public void TestControlString()
        {
            ParseAndAssertControlString("#12", "12");
            ParseAndAssertControlString("#992", "992");
            ParseAndAssertControlString("#29", "29");
        }

        private static void ParseAndAssertQuotedString(string text)
        {
            var token = Parser.ParseToken<CharacterStringToken>(text);
            Assert.AreEqual(1, token.Elements.Count);
            Assert.IsInstanceOf<QuotedStringToken>(token.Elements[0]);

            var quotedToken = (QuotedStringToken) token.Elements[0];
            Assert.AreEqual(Utils.InitialPosition, quotedToken.Start);
            Assert.AreEqual(
                LexingUtils.UpdatePosition(text, Utils.InitialPosition, text.Length - 1), 
                quotedToken.End);
        }
        
        [Test]
        public void TestQuotedString()
        {
            ParseAndAssertQuotedString("'Hello world!'");
            ParseAndAssertQuotedString("' Bye,\ncruel world'");
            ParseAndAssertQuotedString("'Good morning    '");
        }

        private static void ParseAndAssertCompoundString(string text, params Type[] elementTypes)
        {
            var token = Parser.ParseToken<CharacterStringToken>(text);
            Assert.AreEqual(elementTypes.Length, token.Elements.Count);

            Assert.AreEqual(Utils.InitialPosition, token.Start);
            Assert.AreEqual(
                LexingUtils.UpdatePosition(text, Utils.InitialPosition, text.Length - 1), 
                token.End);

            foreach (var (element, type) in token.Elements.Zip(elementTypes))
            {
                Assert.IsInstanceOf(type, element);
            }
        }
        
        [Test]
        public void TestCompoundString()
        {
            ParseAndAssertCompoundString("'Hello'#12'World'", 
                typeof(QuotedStringToken), typeof(ControlStringToken), typeof(QuotedStringToken));
            ParseAndAssertCompoundString("'Hello''World'", 
                typeof(QuotedStringToken), typeof(QuotedStringToken));
            ParseAndAssertCompoundString("'Hello'#12#13'World'", 
                typeof(QuotedStringToken), typeof(ControlStringToken), 
                typeof(ControlStringToken), typeof(QuotedStringToken));
        }
    }
}