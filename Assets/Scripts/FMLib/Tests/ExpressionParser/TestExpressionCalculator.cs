using System.Collections.Generic;
using FMLib.ExpressionParser;
using NUnit.Framework;

namespace FMLib.Tests.ExpressionParser
{
    public class TestExpressionCalculator
    {
        [TestCase("1", 1)]
        [TestCase("1 + 2", 3)]
        [TestCase("-3 + 2", -1)]
        [TestCase("(3+9)/(4-2)", 6)]
        [TestCase("3+2*3-2", 7)]
        [TestCase("2 * 0", 0)]
        [TestCase("!sin(1/2)", 1)]
        public void TestSimple(string expr, float expected)
        {
            var calculator = new ExpressionCalculator(expr);
            var result = calculator.Calculate(new List<VariableMap>());
            Assert.AreEqual(expected, result, 0.001f);
        }

        [TestCase("(x+3) * y", 1, 1, 4)]
        [TestCase("(x+3) * y", 1, 2, 8)]
        [TestCase("(x+3) * y", 2, 1, 5)]
        [TestCase("(x+3) * y", 2, 2, 10)]
        public void TestVariable(string expr, float x, float y, float expected)
        {
            var map = new List<VariableMap>();
            map.Add(new VariableMap("x", x));
            map.Add(new VariableMap("y", y));

            var calculator = new ExpressionCalculator(expr);
            var result = calculator.Calculate(map);

            Assert.AreEqual(expected, result, 0.001f);
        }
    }
}