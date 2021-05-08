using Expressions.Lexing.TokenParsers;
using NUnit.Framework;

namespace ExpressionsTests.Lexing.TokenParsers.CommentParser
{
    public class CommentParserFailedTests
    {
        private static readonly GeneralCommentParser Parser = new();

        [Test]
        public void TestSingleLineComment_FailsWhenNewlineInNested()
        {
            Parser.AssertParsingFails("// Hello world { Oops \n }");
            Parser.AssertParsingFails("// Hello world (* Oops \n )*");
        }

        [Test]
        public void TestSingleLineComment_FailsWhenNestedIncomplete()
        {
            Parser.AssertParsingFails("// Some text (* Another text");
            Parser.AssertParsingFails("// More text { More text");
            Parser.AssertParsingFails("// Bye { (* Hello *) World");
        }
    }
}