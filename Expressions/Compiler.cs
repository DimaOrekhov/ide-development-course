using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace Expressions
{
    public class UniqueVariableCollectorVisitor : IExpressionVisitor
    {
        private readonly ISet<string> _uniqueVariables = new HashSet<string>();

        public ISet<string> UniqueVariables => _uniqueVariables;

        public void Visit(Variable expression) => _uniqueVariables.Add(expression.Name);
    }
    
    public class ExpressionCompilerVisitor : IExpressionVisitor
    {

        private readonly ISet<string> _uniqueVariables;
        private readonly TypeBuilder _typeBuilder;
        private readonly ILGenerator _evaluateGenerator;
        public ExpressionCompilerVisitor(ISet<string> uniqueVariables)
        {
            _uniqueVariables = uniqueVariables;
            
            var aName = new AssemblyName("DynamicAssemblyExample");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(aName.Name);
            _typeBuilder = moduleBuilder.DefineType("Evaluator", TypeAttributes.Public);

            var parameterTypes = new Type[_uniqueVariables.Count];
            for (var i = 0; i < _uniqueVariables.Count; i++)
            {
                parameterTypes[i] = typeof(int);
            }
            
            var evaluateBuilder = _typeBuilder.DefineMethod("Evaluate", MethodAttributes.Public, 
                typeof(int), parameterTypes);
            _evaluateGenerator = evaluateBuilder.GetILGenerator();
        }
        public void Visit(Literal expression) => 
            _evaluateGenerator.Emit(OpCodes.Ldc_I4_S, int.Parse(expression.Value));

        public void Visit(Variable expression)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(BinaryExpression expression)
        {
            expression.Left.Accept(this);
            expression.Right.Accept(this);
            _evaluateGenerator.Emit(expression.Operator switch
            {
                Plus => OpCodes.Add,
                Minus => OpCodes.Sub,
                Mult => OpCodes.Mul,
                Div => OpCodes.Div,
                _ => throw new Exception("Unknown operator type")
            });
        }

        public CompiledExpression Build()
        {
            _evaluateGenerator.Emit(OpCodes.Ret);
            return new CompiledExpression(_typeBuilder.CreateType());
        }
    }

    // Just a wrapper class
    public class CompiledExpression
    {
        public readonly Type Evaluator;
        private readonly MethodInfo _evaluate;
        
        public CompiledExpression(Type evaluator)
        {
            Evaluator = evaluator;
            _evaluate = Evaluator.GetMethod("Evaluate");
        }

        public object Evaluate()
        {
            var obj = Activator.CreateInstance(Evaluator);
            return _evaluate.Invoke(obj, Array.Empty<object>());
        }
    }
    
    public static class Compiler
    {
        public static CompiledExpression CompileExpression(IExpression expression)
        {
            var variableCollector = new UniqueVariableCollectorVisitor();
            expression.Accept(variableCollector);

            var expressionCompiler = new ExpressionCompilerVisitor(variableCollector.UniqueVariables);
            expression.Accept(expressionCompiler);
            
            return expressionCompiler.Build();
        }

        public static CompiledExpression ParseAndCompileExpression(string text) => CompileExpression(Parser.Parse(text));
    }
}