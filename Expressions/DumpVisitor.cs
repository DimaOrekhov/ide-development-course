using System.Text;

namespace Expressions
{
    public class DumpVisitor : IExpressionVisitor
    {
        private readonly StringBuilder _builder;

        private static string Dump(IOperator @operator) => @operator switch
        {
            Plus => "+",
            Minus => "-",
            Mult => "*",
            Div => "/"
        };
        
        public DumpVisitor()
        {
            _builder = new StringBuilder();
        }

        public void Visit(Literal expression)
        {
            _builder.Append("Lit(" + expression.Value + ")");
        }

        public void Visit(Variable expression)
        {
            _builder.Append("Var(" + expression.Name + ")");
        }

        public void Visit(BinaryExpression expression)
        {
            _builder.Append("Bin(");
            expression.Left.Accept(this);
            _builder.Append(Dump(expression.Operator));
            expression.Right.Accept(this);
            _builder.Append(')');
        }

        public void Visit(ParenExpression expression)
        {
            _builder.Append("Par(");
            expression.Operand.Accept(this);
            _builder.Append(')');
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}