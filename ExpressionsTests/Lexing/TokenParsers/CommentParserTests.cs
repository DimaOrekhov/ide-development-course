using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Expressions.Lexing;
using Expressions.Lexing.TokenParsers;
using Expressions.Lexing.Tokens;
using NUnit.Framework;

namespace ExpressionsTests.Lexing.TokenParsers
{
    public abstract class CommentTypeTree
    {
        protected CommentTypeTree()
        {
        }
        
        protected abstract Type NodeType
        {
            get;
        }

        protected abstract List<CommentTypeTree> Children
        {
            get;
        }

        public void AssertIsInstance(CommentToken token)
        {
            Assert.IsInstanceOf(NodeType, token);
            Assert.AreEqual(Children.Count, token.NestedComments.Count);
            
            foreach (var (typeTree, nestedToken) in Children.Zip(token.NestedComments))
            {
                typeTree.AssertIsInstance(nestedToken);
            }
        }
    }
    
    public class CommentTypeNode<T> : CommentTypeTree where T : CommentToken
    {
        protected override Type NodeType => typeof(T);

        protected override List<CommentTypeTree> Children { get; }

        public CommentTypeNode(params CommentTypeTree[] children)
        {
            Children = children.ToList();
        }
    }
    
    public class CommentParserTests
    {
        private static readonly GeneralCommentParser Parser = new();

        private static void SimpleCommentTest<T>(string text, CommentTypeTree typeTree = null) where T : CommentToken
        {
            var token = Parser.ParseToken<T>(text);
            Assert.AreEqual(Utils.InitialPosition, token.Start);
            Assert.AreEqual(LexingUtils.UpdatePosition(text, Utils.InitialPosition, text.Length - 1), token.End);

            typeTree?.AssertIsInstance(token);
        }

        [Test]
        public void TestSingleLineComment() => SimpleCommentTest<LineComment>("// Some comment text\n");

        [Test]
        public void TestSingleLineComment_Eof() => SimpleCommentTest<LineComment>("// Another text inside comment");
        
        [Test]
        public void TestSingleLineComment_Nested() => SimpleCommentTest<LineComment>(
            "// Another text inside { Hello } some text, (* Nested 2 *) abc", 
            new CommentTypeNode<LineComment>(
                new CommentTypeNode<CurlyBracketComment>(), new CommentTypeNode<RoundBracketComment>()
                ));
        
        [Test]
        public void TestSingleLineComment_NestedSingleLine() => SimpleCommentTest<LineComment>(
            "// Another text inside { Hello } some text, (* Nested 2 *) // abc", 
            new CommentTypeNode<LineComment>(
                new CommentTypeNode<CurlyBracketComment>(), new CommentTypeNode<RoundBracketComment>(), new CommentTypeNode<LineComment>()
            ));

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

        [Test]
        public void TestRoundComment_SingleLine() => SimpleCommentTest<RoundBracketComment>("(* What a great day today is *)");
        
        [Test]
        public void TestRoundComment_MultiLine() => SimpleCommentTest<RoundBracketComment>(Utils.JoinLines(
            "(*", 
            "  Hello", 
            "  it is me", 
            "  how are you?", 
            "*)"
            ));
        
        [Test]
        public void TestRoundComment_Nested() => SimpleCommentTest<RoundBracketComment>(Utils.JoinLines(
            "(*", 
            "  Some text { Hello",
            "              World }",
            "  Wow // it is me",
            "  how are you? (*",
            "  I am fine *)",
            "*)"), 
            new CommentTypeNode<RoundBracketComment>(
            new CommentTypeNode<CurlyBracketComment>(), new CommentTypeNode<LineComment>(), new CommentTypeNode<RoundBracketComment>()
        ));
        
        [Test]
        public void TestCurlyComment_SingleLine() => SimpleCommentTest<CurlyBracketComment>("{ What a great day today is }");

        [Test]
        public void TestCurlyComment_MultiLine() => SimpleCommentTest<CurlyBracketComment>(Utils.JoinLines(
            "{", 
            "  I am fine!", 
            "  And what about you?", 
            "}"
            ));
    }
}