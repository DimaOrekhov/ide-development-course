using System;
using System.Collections.Generic;
using Expressions;
using NUnit.Framework;

namespace ExpressionsTests
{
    public class EvaluationTests
    {
        private static int Evaluate(string text, Dictionary<string, int> environment = null) =>
            Parser.Parse(text).Evaluate(environment);
        
        [Test]
        public void TestTwoOperands()
        {
            Assert.AreEqual(3, Evaluate("1 + 2"));
            Assert.AreEqual(2, Evaluate("8 - 6  "));
            Assert.AreEqual(14, Evaluate(" 7 *  2"));
            Assert.AreEqual(3, Evaluate("9 / 3"));
        }

        [Test]
        public void TestTwoOperandsInParens()
        {
            Assert.AreEqual(3, Evaluate("(1 + 2)"));
        }

        [Test]
        public void TestSingleLiteralInParen()
        {
            Assert.AreEqual(12, Evaluate("(3) * (4)"));
            Assert.AreEqual(12, Evaluate("((3) * (4) )"));
        }

        [Test]
        public void TestLeftAssociativity()
        {
            Assert.AreEqual(9 - 2 - 3,Evaluate("9 - 2 - 3"));
            Assert.AreEqual(8 / 2 / 2, Evaluate("8 / 2 / 2"));
        }

        [Test]
        public void TestParenPriority()
        {
            Assert.AreEqual((3 + 4) * 2, 
                Evaluate("(3 + 4) * 2"));
            Assert.AreEqual(3 + 4 * 2, 
                Evaluate("3 + 4 * 2"));
        }

        [Test]
        public void TestComplicatedExpression()
        {
            Assert.AreEqual(1 + (3) * 4 / ( 3 -  1 - 1), 
                Evaluate("  1 + (3) * 4 / ( 3 -  1 - 1)  "));
        }
    }
}