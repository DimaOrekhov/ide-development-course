using Expressions;
using Expressions.Parsing;
using NUnit.Framework;

namespace ExpressionsTests
{
    public class ParsingTests
    {
        private static string Dump(string text)
        {
            var visitor = new DumpVisitor();
            Parser.Parse(text).Accept(visitor);
            return visitor.ToString();
        }

        [Test]
        public void TestTwoOperands()
        {
            Assert.AreEqual("Bin(Lit(1)+Lit(5))", Dump("1 + 5"));
            Assert.AreEqual("Bin(Lit(3)-Lit(0))", Dump("3 - 0"));
            Assert.AreEqual("Bin(Var(a)*Lit(4))", Dump("a * 4"));
            Assert.AreEqual("Bin(Lit(9)/Var(b))", Dump("9 / b"));
        }

        [Test]
        public void TestTwoOperandsInParen()
        {
            Assert.AreEqual("Par(Bin(Lit(1)+Var(c)))", Dump("(1 + c)"));
        }

        [Test]
        public void TestParenChangesPriority()
        {
            Assert.AreEqual("Bin(Bin(Lit(1)*Var(x))+Lit(3))", Dump("1 * x + 3"));
            Assert.AreEqual("Bin(Lit(1)*Par(Bin(Var(x)+Lit(3))))", Dump("1 * (x + 3)"));
        }
    }
}