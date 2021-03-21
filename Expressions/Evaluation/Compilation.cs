using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Expressions.Parsing;

namespace Expressions.Evaluation
{
    public class UniqueVariableCollectorVisitor : IExpressionVisitor
    {
        private readonly ISet<string> _uniqueVariables = new SortedSet<string>();

        public ISet<string> UniqueVariables => _uniqueVariables;

        public void Visit(Variable expression) => _uniqueVariables.Add(expression.Name);
    }
    
    public class ExpressionCompilerVisitor : IExpressionVisitor
    {

        private readonly Dictionary<string, byte> _variablesMap;
        private readonly TypeBuilder _typeBuilder;
        private readonly ILGenerator _evaluateGenerator;

        public static Dictionary<string, byte> SetToIndexMap(ISet<string> variables)
        {
            variables = variables is SortedSet<string> ? variables : new SortedSet<string>(variables);
            
            return variables.Select((name, index) => (name, index))
                .GroupBy(pair => pair.name)
                .ToDictionary(
                    group => group.Key, 
                    group => (byte) group.First().index);
        }


        public ExpressionCompilerVisitor(ISet<string> variables)
        {
            if (variables.Count > 254)
            {
                throw new ArgumentException("Expression contains too many variables");
            }
            
            _variablesMap = variables switch
            {
                SortedSet<string> sortedSet => SetToIndexMap(sortedSet),
                _ => SetToIndexMap(new SortedSet<string>(variables))
            };
            
            var aName = new AssemblyName("DynamicAssemblyExample");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(aName.Name);
            _typeBuilder = moduleBuilder.DefineType("Evaluator", TypeAttributes.Public);

            var parameterTypes = new Type[variables.Count];
            for (var i = 0; i < variables.Count; i++)
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
            _evaluateGenerator.Emit(OpCodes.Ldarg_S, _variablesMap[expression.Name] + 1);
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

    public class CompiledExpression
    {
        public readonly Type Evaluator;
        private readonly MethodInfo _evaluate;
        
        public CompiledExpression(Type evaluator)
        {
            Evaluator = evaluator;
            _evaluate = Evaluator.GetMethod("Evaluate");
        }

        public int Evaluate(Dictionary<string, int> environment = null)
        {
            object[] parameters;
            
            if (environment != null)
            {
                parameters = new object [environment.Count];
                var varToIndex = ExpressionCompilerVisitor.SetToIndexMap(environment.Keys.ToHashSet());

                foreach (var varName in environment.Keys)
                {
                    var varIndex = varToIndex[varName];
                    parameters[varIndex] = environment[varName];
                }
            }
            else
            {
                parameters = Array.Empty<object>();
            }
            
            var obj = Activator.CreateInstance(Evaluator);
            return (int) _evaluate.Invoke(obj, parameters);
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