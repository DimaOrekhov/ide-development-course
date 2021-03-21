namespace Expressions
{
    public interface IExpressionVisitor
    {
        void Visit(Literal expression)
        {
            
        }

        void Visit(Variable expression)
        {
            
        }

        void Visit(BinaryExpression expression)
        {
            expression.Left.Accept(this);
            expression.Right.Accept(this);
        }

        void Visit(ParenExpression expression)
        {
            expression.Operand.Accept(this);
        }
    }
}