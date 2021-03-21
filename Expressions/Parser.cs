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
            _ => throw ParsingException.UnknownTokenType(previous)
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
            _ => throw ParsingException.UnknownTokenType(previous)
        };

        public static IOperator AsOperator(this OperatorToken token) => token.Value switch
        {
            "+" => new Plus(),
            "-" => new Minus(),
            "*" => new Mult(),
            "/" => new Div(),
            _ => throw ParsingException.UnknownOperatorToken(token)
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

            var openingParenToken = new OpeningParenToken(-1);
            opStack.Push(new OperatorOrParen(openingParenToken));

            Token prevToken = openingParenToken;
            var numberOfUnmatchedParen = 1;
            foreach (var token in new LexedString(text))
            {
                if (!prevToken.CanBeFollowedBy(token))
                {
                    throw ParsingException.UnexpectedToken(token);
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
                        ProcessOperator(exprStack, opStack, op.AsOperator());
                        break;
                    case OpeningParenToken p:
                        numberOfUnmatchedParen++;
                        opStack.Push(new OperatorOrParen(p));
                        break;
                    case ClosingParenToken:
                        numberOfUnmatchedParen--;
                        if (numberOfUnmatchedParen < 0)
                        {
                            throw new Exception("Incorrect bracket sequence");
                        }

                        CloseParen(exprStack, opStack);
                        break;
                    default: throw ParsingException.UnknownTokenType(token);
                }

                prevToken = token;
            }

            ValidateEndOfParsing(prevToken, --numberOfUnmatchedParen);
            CloseParen(exprStack, opStack);

            return exprStack.Peek();
        }

        private static void ProcessOperator(Stack<IExpression> exprStack, Stack<OperatorOrParen> opStack, 
            IOperator newOperator)
        {
            var top = opStack.Peek();
            switch (top.Paren)
            {
                case null: break;
                case OpeningParenToken:
                    opStack.Push(new OperatorOrParen(newOperator));
                    return;
                case ClosingParenToken: throw new Exception("Illegal state");
            }

            switch (top.Operator)
            {
                case null: break;
                default:
                    if (top.Operator.Priority >= newOperator.Priority)
                    {
                        opStack.Pop();
                        ReduceTopExpressions(exprStack, top.Operator);
                    }
                    opStack.Push(new OperatorOrParen(newOperator));
                    break;
            }
        }

        private static void ReduceTopExpressions(Stack<IExpression> exprStack, IOperator @operator)
        {
            var right = exprStack.Pop();
            var left = exprStack.Pop();

            exprStack.Push(new BinaryExpression(left, @operator, right));
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
                    case OpeningParenToken:
                        exprStack.Push(new ParenExpression(exprStack.Pop()));
                        return;
                    case ClosingParenToken: throw new Exception("Illegal state");
                }
            } while (opStack.Count > 0);
        }

        private static void ValidateEndOfParsing(Token lastToken, int numberOfUnmatchedParen)
        {
            if (lastToken is OperatorToken)
            {
                throw ParsingException.UnexpectedEof();
            }

            if (numberOfUnmatchedParen != 0)
            {
                throw new Exception("Incorrect bracket sequence");
            }
        }
    }
}