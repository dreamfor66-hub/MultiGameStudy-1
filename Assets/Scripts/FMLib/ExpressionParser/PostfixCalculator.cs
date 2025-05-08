using System;
using System.Collections.Generic;

namespace FMLib.ExpressionParser
{
    public class PostfixCalculator
    {
        private readonly List<IToken> postfix;
        private readonly List<VariableMap> variableMap;

        public PostfixCalculator(List<IToken> postfix, List<VariableMap> variableMap)
        {
            this.postfix = postfix;
            this.variableMap = variableMap;
        }

        public float Calculate()
        {
            var valueStack = new Stack<float>();
            foreach (var token in postfix)
            {
                if (token is ValueToken value)
                {
                    valueStack.Push(value.Value);
                }
                else if (token is VariableToken variable)
                {
                    valueStack.Push(GetVariable(variable));
                }
                else if (token is OperatorToken op)
                {
                    if (op is BinaryOperatorToken binary)
                    {
                        var val2 = valueStack.Pop();
                        var val1 = valueStack.Pop();
                        valueStack.Push(binary.Calculate(val1, val2));
                    }
                    else if (op is UnaryOperatorToken unary)
                    {
                        var val = valueStack.Pop();
                        valueStack.Push(unary.Calculate(val));
                    }
                }
            }

            if (valueStack.Count == 1)
            {
                return valueStack.Pop();
            }
            else
            {
                throw new ArgumentException("parsing error");
            }
        }

        private float GetVariable(VariableToken variable)
        {
            var find = variableMap.Find(x => x.Key == variable.Expr);
            if (find != null)
                return find.Value;
            else
                throw new ArgumentException($"unknown variable : {variable.Expr}");

        }
    }
}