using System.Collections.Generic;
using Expressions;
using NUnit.Framework;

namespace ExpressionsTests
{
    public abstract class AbstractEvaluationTest
    {
        protected abstract int Evaluate(string text, Dictionary<string, int> environment = null);

        [Test]
        public void TestTwoOperands()
        {
            Assert.AreEqual(3, Evaluate("1 + 2"));
            Assert.AreEqual(2, Evaluate("8 - 6  "));
            Assert.AreEqual(14, Evaluate(" 7 *  2"));
            Assert.AreEqual(3, Evaluate("9 / 3"));
        }

        [Test]
        public void TestTwoOperandsWithVariables()
        {
            var env = new Dictionary<string, int>
            {
                { "a", 10 },
                { "b", 2  }
            };
            Assert.AreEqual(12, Evaluate("a + b", env));
            Assert.AreEqual(-8, Evaluate("b - a", env));
            Assert.AreEqual(8, Evaluate("a - b", env));
            Assert.AreEqual(20, Evaluate("a *  b", env));
            Assert.AreEqual(5, Evaluate("a / b", env));
            Assert.AreEqual(0, Evaluate("b / a", env));

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
        public void TestLeftAssociativityWithVariables()
        {
            var env = new Dictionary<string, int>
            {
                { "a", 8  },
                { "b", 3  },
                { "c", 24 }
            };
            Assert.AreEqual(env["b"] - env["a"] - env["c"], 
                Evaluate("b - a - c", env));
            Assert.AreEqual(env["c"] / env["a"] / env["b"], 
                Evaluate("c / a / b", env));
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

        [Test]
        public void TestComplicatedExpressionWithVariables()
        {
            var env = new Dictionary<string, int>
            {
                { "a", 8   },
                { "c", 3   },
                { "x", 24  },
                { "y", 997 }
            };
            Assert.AreEqual(env["c"] + (env["x"]) * 4 / (env["y"] - env["a"] - 1),
                Evaluate("  c + (x) * 4 / ( y -  a - 2)  ", env));
        }
    }
    
    public class InterpretationTests : AbstractEvaluationTest
    {
        protected override int Evaluate(string text, Dictionary<string, int> environment = null) =>
            Parser.Parse(text).Evaluate(environment);
    }

    public class CompilationTests : AbstractEvaluationTest
    {
        protected override int Evaluate(string text, Dictionary<string, int> environment = null) =>
            Compiler.ParseAndCompileExpression(text).Evaluate(environment);
    }
}