namespace Expressions
{
    public interface IExpression
    {
        void Accept(IExpressionVisitor visitor);
    }
    
    
    public class Literal : IExpression
    {
        public Literal(string value)
        {
            Value = value;
        }

        public readonly string Value;
        
        public void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class Variable : IExpression
    {
        public Variable(string name)
        {
            Name = name;
        }

        public readonly string Name;
        public void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    
    public class BinaryExpression : IExpression
    {
        public readonly IExpression FirstOperand;
        public readonly IExpression SecondOperand;
        public readonly string Operator;

        public BinaryExpression(IExpression firstOperand, IExpression secondOperand, string @operator)
        {
            FirstOperand = firstOperand;
            SecondOperand = secondOperand;
            Operator = @operator;
        }

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    
    public class ParenExpression : IExpression
    {
        public ParenExpression(IExpression operand)
        {
            Operand = operand;
        }

        public readonly IExpression Operand;
        public void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}