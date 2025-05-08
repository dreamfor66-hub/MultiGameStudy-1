using System.Collections.Generic;

namespace FMLib.ExpressionParser
{
    public class ExpressionCalculator
    {
        private List<IToken> postfix;

        public ExpressionCalculator(string expr)
        {
            var tokens = new Tokenizer(expr).Tokenize();
            postfix = new ParseToPostfix(tokens).Parse();
        }

        public float Calculate(List<VariableMap> map)
        {
            var calculator = new PostfixCalculator(postfix, map);
            return calculator.Calculate();
        }
    }
}