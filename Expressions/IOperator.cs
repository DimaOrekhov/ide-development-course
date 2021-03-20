namespace Expressions
{
    public interface IOperator
    {
        int Priority
        {
            get;
        }
    }
    public class Plus : IOperator
    {
        public int Priority => 0;
    }

    public class Minus : IOperator
    {
        public int Priority => 0;
    }

    public class Mult : IOperator
    {
        public int Priority => 1;
    }

    public class Div : IOperator
    {
        public int Priority => 1;
    }
}