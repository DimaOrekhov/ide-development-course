using System.Collections.Generic;

namespace Expressions.Evaluation
{
    public static class Interpreter
    {
        public static int Evaluate(this IExpression expression, Dictionary<string, int> environment) => expression switch
        {
            Literal lit => int.Parse(lit.Value),
            Variable var => environment[var.Name],
            BinaryExpression bin => bin.Evaluate(environment),
            ParenExpression paren => paren.Operand.Evaluate(environment)
        };

        private static int Evaluate(this BinaryExpression expression, Dictionary<string, int> environment)
        {
            var left = expression.Left.Evaluate(environment);
            var right = expression.Right.Evaluate(environment);
            return expression.Operator switch
            {
                Plus => left + right,
                Minus => left - right,
                Mult => left * right,
                Div => left / right
            };
        }
    }
}