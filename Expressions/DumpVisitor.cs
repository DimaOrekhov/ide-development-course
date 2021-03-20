using System.Text;

namespace Expressions
{
    public class DumpVisitor : IExpressionVisitor
    {
        private readonly StringBuilder _builder;

        public DumpVisitor()
        {
            _builder = new StringBuilder();
        }

        public void Visit(Literal expression)
        {
            _builder.Append("Literal(" + expression.Value + ")");
        }

        public void Visit(Variable expression)
        {
            _builder.Append("Variable(" + expression.Name + ")");
        }

        public void Visit(BinaryExpression expression)
        {
            _builder.Append("Binary(");
            expression.FirstOperand.Accept(this);
            _builder.Append(expression.Operator);
            expression.SecondOperand.Accept(this);
            _builder.Append(')');
        }

        public void Visit(ParenExpression expression)
        {
            _builder.Append("Paren(");
            expression.Operand.Accept(this);
            _builder.Append(')');
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}