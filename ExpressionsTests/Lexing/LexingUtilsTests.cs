using System;
using Expressions.Lexing;
using Expressions.Lexing.Tokens;
using NUnit.Framework;

namespace ExpressionsTests
{
    public class LexingUtilsTests
    {
        private static readonly Position Initial = new(0, 0, 0);
        
        [Test]
        public void Test()
        {
            const string text = "x\n    y";
            var xPosition = LexingUtils.UpdatePosition(text, Initial, 0);
            Assert.AreEqual(Initial, xPosition);

            var newLinePosition = LexingUtils.UpdatePosition(text, xPosition, 1);
            Assert.AreEqual(new Position(0, 1, 1), newLinePosition);
            
            Assert.AreEqual(newLinePosition, LexingUtils.UpdatePosition(text, Initial, 1));

            var yPosition = LexingUtils.UpdatePosition(text, newLinePosition, 6);
            Assert.AreEqual(new Position(1, 4, 6), yPosition);
            Assert.AreEqual(yPosition, LexingUtils.UpdatePosition(text, Initial, 6));
        }
    }
}