
using System;
using System.Xml.XPath;
using NUnit.Framework;
using FMLib.ExpressionParser;

namespace FMLib.Tests.ExpressionParser
{

    public class TestTokenizer
    {
        [TestCase("123", 123)]
        [TestCase("1.2", 1.2f)]
        [TestCase("0.0", 0f)]
        [TestCase("03.20", 3.2f)]
        public void TestNumber(string expr, float expected)
        {
            var tokenizer = new Tokenizer(expr);
            var tokens = tokenizer.Tokenize();
            Assert.AreEqual(((ValueToken)tokens[0]).Value, expected, 0.01);
        }

        [TestCase("+", typeof(PlusToken))]
        [TestCase("-", typeof(MinusToken))]
        [TestCase("*", typeof(MultiplyToken))]
        [TestCase("/", typeof(DivideToken))]
        [TestCase("!sin", typeof(SinToken))]
        public void TestOperator(string expr, Type type)
        {
            var tokenizer = new Tokenizer(expr);
            var tokens = tokenizer.Tokenize();
            Assert.True(type.IsInstanceOfType(tokens[0]));
        }

        [TestCase("(", true)]
        [TestCase(")", false)]
        public void TestBracket(string expr, bool expected)
        {
            var tokenizer = new Tokenizer(expr);
            var tokens = tokenizer.Tokenize();
            Assert.AreEqual(((BracketToken)tokens[0]).IsOpen, expected);
        }

        [TestCase("x")]
        [TestCase("var")]
        public void TestVariable(string expr)
        {
            var tokenizer = new Tokenizer(expr);
            var tokens = tokenizer.Tokenize();
            Assert.AreEqual(expr, ((VariableToken)tokens[0]).Expr);
        }

        [Test]
        public void TestComplex()
        {
            var tokenizer = new Tokenizer("(x + 3) / y");
            var tokens = tokenizer.Tokenize();
            Assert.AreEqual(tokens.Count, 7);
            Assert.AreEqual(((BracketToken)tokens[0]).IsOpen, true);
            Assert.AreEqual(((VariableToken)tokens[1]).Expr, "x");
            Assert.True(tokens[2] is PlusToken);
            Assert.AreEqual(((ValueToken)tokens[3]).Value, 3f, 0.01f);
            Assert.AreEqual(((BracketToken)tokens[4]).IsOpen, false);
            Assert.True(tokens[5] is DivideToken);
            Assert.AreEqual(((VariableToken)tokens[6]).Expr, "y");
        }
    }
}
