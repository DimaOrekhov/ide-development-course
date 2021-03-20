using System;
using System.Collections.Generic;

namespace Expressions
{
    public static class TokenExtensions
    {
        public static bool CanBeFollowedBy(this Token previous, Token next) => previous switch
        {
            OperatorToken op => op.CanBeFollowedBy(next),
            VariableToken var => var.CanBeFollowedBy(next),
            LiteralToken lit => lit.CanBeFollowedBy(next),
            ParenToken p => p.CanBeFollowedBy(next),
            _ => throw new Exception("Unknown token type")
        };

        private static bool CanBeFollowedBy(this OperatorToken previous, Token next) =>
            next is LiteralToken || next is VariableToken || next is OpeningParenToken;

        private static bool CanBeFollowedBy(this VariableToken previous, Token next) =>
            next is OperatorToken || next is ClosingParenToken;

        private static bool CanBeFollowedBy(this LiteralToken previous, Token next) =>
            next is OperatorToken || next is ClosingParenToken;

        private static bool CanBeFollowedBy(this ParenToken previous, Token next) => previous switch
        {
            OpeningParenToken => next is not OperatorToken,
            ClosingParenToken => next is OperatorToken || next is ClosingParenToken,
            _ => throw new Exception("Unknown token type")
        };

        public static IOperator AsOperator(this OperatorToken token) => token.Value switch
        {
            "+" => new Plus(),
            "-" => new Minus(),
            "*" => new Mult(),
            "/" => new Div(),
            _ => throw new Exception("Unknown operator token")
        };
    }

    public static class Parser
    {
        private record OperatorOrParen
        {
            public readonly IOperator Operator;
            public readonly ParenToken Paren;
            public OperatorOrParen(IOperator op)
            {
                Operator = op;
                Paren = null;
            }

            public OperatorOrParen(ParenToken paren)
            {
                Operator = null;
                Paren = paren;
            }

            public bool IsOperator() => Operator != null;

            public bool IsParen() => Paren != null;
        }

        public static IExpression Parse(string text)
        {
            var exprStack = new Stack<IExpression>();
            var opStack = new Stack<OperatorOrParen>();

            var openingParenToken = new OpeningParenToken();
            opStack.Push(new OperatorOrParen(openingParenToken));

            Token prevToken = openingParenToken;
            foreach (var token in new LexedString(text))
            {
                if (!prevToken.CanBeFollowedBy(token))
                {
                    throw new Exception("Unexpected token: " + token.Value);
                }
                
                switch (token)
                {
                    case LiteralToken lit:
                        exprStack.Push(new Literal(lit.Value));
                        break;
                    case VariableToken var:
                        exprStack.Push(new Variable(var.Value));
                        break;
                    case OperatorToken op:
                        opStack.Push(new OperatorOrParen(op.AsOperator()));
                        break;
                    case OpeningParenToken p:
                        opStack.Push(new OperatorOrParen(p));
                        break;
                    case ClosingParenToken:
                        CloseParen(exprStack, opStack);
                        break;
                    default: throw new Exception("Unknown token type: ");
                }

                prevToken = token;
            }

            CloseParen(exprStack, opStack);

            if (exprStack.Count > 1 || opStack.Count != 0)
            {
                throw new Exception();
            }
            
            return exprStack.Peek();
        }

        private static void CloseParen(Stack<IExpression> exprStack, Stack<OperatorOrParen> opStack)
        {
            do
            {
                var current = opStack.Pop();
                
                if (current.IsOperator())
                {
                    var right = exprStack.Pop();
                    var left = exprStack.Pop();
                    
                    exprStack.Push(new BinaryExpression(left, current.Operator, right));
                    continue;
                }

                switch (current.Paren)
                {
                    case ClosingParenToken: throw new Exception("Incorrect bracket sequence");
                    case OpeningParenToken: return;
                }
            } while (opStack.Count > 0);
        }
    }
}