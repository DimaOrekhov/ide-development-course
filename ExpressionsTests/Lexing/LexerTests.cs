using System;
using System.Collections.Generic;
using System.Linq;
using Expressions.Lexing;
using Expressions.Lexing.TokenParsers;
using Expressions.Lexing.Tokens;
using NUnit.Framework;

namespace ExpressionsTests.Lexing
{
    public class LexerTests
    {
        private void AssertTokenTypes(IReadOnlyCollection<Token> tokens, IReadOnlyCollection<Type> types)
        {
            Assert.AreEqual(types.Count, tokens.Count);
            foreach (var (token, type) in tokens.Zip(types))
            {
                Assert.IsInstanceOf(type, token);
            }
        }

        private void ParseAndAssertTypes(string text, IReadOnlyCollection<Type> types) => 
            AssertTokenTypes(Lexer.Lex(text, 0), types);

        private void ParseAndAssertTypes(string text, params Type[] types) =>
            ParseAndAssertTypes(text, types.ToList());

        [Test]
        public void TestAssignment()
        {
            ParseAndAssertTypes("x := y+z;",
                typeof(IdentifierToken), typeof(WhiteSpaceToken), 
                typeof(SpecialSymbolToken), typeof(WhiteSpaceToken), 
                typeof(IdentifierToken), typeof(SpecialSymbolToken), typeof(IdentifierToken),
                typeof(SpecialSymbolToken)
            );
        }

        [Test]
        public void TestForLoop()
        {
            ParseAndAssertTypes("for i:=1 to 10 do writeln(i);", 
                typeof(IdentifierToken), typeof(WhiteSpaceToken), 
                typeof(IdentifierToken), typeof(SpecialSymbolToken), typeof(NumberToken),
                typeof(WhiteSpaceToken), typeof(IdentifierToken), typeof(WhiteSpaceToken), typeof(NumberToken), 
                typeof(WhiteSpaceToken), typeof(IdentifierToken), typeof(WhiteSpaceToken),
                typeof(IdentifierToken), typeof(SpecialSymbolToken), typeof(IdentifierToken), typeof(SpecialSymbolToken),
                typeof(SpecialSymbolToken));
        }
        
        [Test]
        public void TestHelloWorld() => ParseAndAssertTypes(Programs.HelloWorld, Programs.HelloWorldTypes);

        [Test]
        public void TestHelloWorldWithComment() =>
            ParseAndAssertTypes(Programs.HelloWorldWithComment, Programs.HelloWorldWithCommentTypes);
    }
    
    public static class Programs
    {
        public static readonly string HelloWorld = @"program Hello;
begin
  writeln('Hello, world.');
end.";

        public static readonly List<Type> HelloWorldTypes = new List<Type>
        {
            // 1 line
            typeof(IdentifierToken), typeof(WhiteSpaceToken), typeof(IdentifierToken), typeof(SpecialSymbolToken), 
            typeof(WhiteSpaceToken),
            // 2 line
            typeof(IdentifierToken), typeof(WhiteSpaceToken),
            // 3 line
            typeof(IdentifierToken), typeof(SpecialSymbolToken), typeof(CharacterStringToken), 
            typeof(SpecialSymbolToken), typeof(SpecialSymbolToken), typeof(WhiteSpaceToken),
            // 4 line
            typeof(IdentifierToken), typeof(SpecialSymbolToken)
        };

        
        public static readonly string HelloWorldWithComment = @"program Hello;
{
  This is a Hello World Program
  // Pascal Language
}
begin
  writeln('Hello, world.');
end.";

        public static readonly List<Type> HelloWorldWithCommentTypes = new List<Type>
        {
            // 1 line
            typeof(IdentifierToken), typeof(WhiteSpaceToken), typeof(IdentifierToken), typeof(SpecialSymbolToken), 
            typeof(WhiteSpaceToken),
            // 2-5 line
            typeof(CurlyBracketComment), typeof(WhiteSpaceToken),
            // 6 line
            typeof(IdentifierToken), typeof(WhiteSpaceToken),
            // 7 line
            typeof(IdentifierToken), typeof(SpecialSymbolToken), typeof(CharacterStringToken), 
            typeof(SpecialSymbolToken), typeof(SpecialSymbolToken), typeof(WhiteSpaceToken),
            // 8 line
            typeof(IdentifierToken), typeof(SpecialSymbolToken)
        };
    }
}