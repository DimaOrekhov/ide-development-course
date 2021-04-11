using System.Collections.Generic;
using System.Linq;
using Expressions.Lexing;
using Expressions.Lexing.Tokens;
using NUnit.Framework;

namespace ExpressionsTests
{
    public static class TokenDumpExtensions
    {
        public static string Dump(this ElementaryToken tok) => tok switch
        {
            OperatorToken op => op.Dump(),
            IdentifierToken var => var.Dump(),
            LiteralToken lit => lit.Dump(),
            ParenToken p => p.Dump(),
        };

        private static string Dump(this OperatorToken tok) => $"Op({tok.Value})";

        private static string Dump(this IdentifierToken tok) => $"Id({tok.Value})";

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
        public void TestTwoSingleCharIdOp()
        {
            Assert.AreEqual(new List<string> {"Id(a)", "Op(+)", "Id(b)"}, AsTokenStrings("a + b"));
            Assert.AreEqual(new List<string> {"Id(x)", "Op(-)", "Id(z)"}, AsTokenStrings("x - z"));
            Assert.AreEqual(new List<string> {"Id(i)", "Op(*)", "Id(c)"}, AsTokenStrings("i * c"));
            Assert.AreEqual(new List<string> {"Id(k)", "Op(/)", "Id(j)"}, AsTokenStrings("k / j"));
        }

        [Test]
        public void TestTwoIdOp()
        {
            Assert.AreEqual(new List<string> {"Id(_xyz)", "Op(+)", "Id(snake_case_var)"}, 
                AsTokenStrings("_xyz + snake_case_var"));
            Assert.AreEqual(new List<string> {"Id(camelCaseVar)", "Op(-)", "Id(SCREAMING_SNAKE_CASE_VAR)"}, 
                AsTokenStrings("camelCaseVar - SCREAMING_SNAKE_CASE_VAR"));
            Assert.AreEqual(new List<string> {"Id(_)", "Op(-)", "Id(_)"}, 
                AsTokenStrings("_ - _"));
        }
        
        [Test]
        public void TestEmptyString()
        {
            Assert.AreEqual(new List<string>(), AsTokenStrings(""));
            Assert.AreEqual(new List<string>(), AsTokenStrings("   "));
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