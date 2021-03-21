using System.Collections.Generic;
using System.Linq;
using Expressions;
using NUnit.Framework;

namespace ExpressionsTests
{
    public static class TokenDumpExtensions
    {
        public static string Dump(this Token tok) => tok switch
        {
            OperatorToken op => op.Dump(),
            VariableToken var => var.Dump(),
            LiteralToken lit => lit.Dump(),
            ParenToken p => p.Dump(),
        };

        private static string Dump(this OperatorToken tok) => $"Op({tok.Value})";

        private static string Dump(this VariableToken tok) => $"Var({tok.Value})";

        private static string Dump(this LiteralToken tok) => $"L({tok.Value})";

        private static string Dump(this ParenToken tok) => $"Par({tok.Value})";
    }
    
    public class LexingTests
    {
        private static List<string> AsTokenStrings(string text) => 
            new LexedString(text).Select(token => token.Dump()).ToList();

        [Test]
        public void TestTwoLiteralOp()
        {
            Assert.AreEqual(new List<string> {"L(1)", "Op(+)", "L(2)"}, AsTokenStrings("1 + 2"));
            Assert.AreEqual(new List<string> {"L(3)", "Op(-)", "L(7)"}, AsTokenStrings("3 - 7"));
            Assert.AreEqual(new List<string> {"L(8)", "Op(*)", "L(1)"}, AsTokenStrings("8 * 1"));
            Assert.AreEqual(new List<string> {"L(6)", "Op(/)", "L(0)"}, AsTokenStrings("6 / 0"));
        }

        [Test]
        public void TestComplicatedExpression()
        {
            Assert.AreEqual(new List<string>
            {
                "L(1)", "Op(+)", "Par(()", "L(3)", "Par())",
                "Op(*)", "L(4)", "Op(/)", "Par(()", "L(3)",
                "Op(-)", "L(1)", "Par())"
            }, AsTokenStrings("  1 + (3) * 4 / ( 3 -  1)  "));
        }
    }
}