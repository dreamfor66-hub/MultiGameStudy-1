using System.Runtime.Remoting.Messaging;
using Sirenix.Utilities;
using UnityEngine;

namespace FMLib.ExpressionParser
{
    public interface IToken
    {

    }

    public abstract class OperandToken : IToken
    {

    }

    public class VariableToken : OperandToken
    {
        public string Expr { get; }

        public VariableToken(string expr)
        {
            Expr = expr;
        }
    }

    public class ValueToken : OperandToken
    {
        public float Value { get; }

        public ValueToken(float value)
        {
            Value = value;
        }
    }

    public class BracketToken : IToken
    {
        public bool IsOpen { get; }

        public BracketToken(bool isOpen)
        {
            IsOpen = isOpen;
        }
    }

    public abstract class OperatorToken : IToken
    {
        public abstract int Priority { get; }
    }

    public abstract class BinaryOperatorToken : OperatorToken
    {
        public abstract float Calculate(float value1, float value2);
    }

    public abstract class UnaryOperatorToken : OperatorToken
    {
        public abstract float Calculate(float value);
    }

    public class PlusToken : BinaryOperatorToken
    {
        public override int Priority => 1;
        public override float Calculate(float value1, float value2) => value1 + value2;
    }

    public class MinusToken : BinaryOperatorToken
    {
        public override int Priority => 1;
        public override float Calculate(float value1, float value2) => value1 - value2;
    }

    public class MultiplyToken : BinaryOperatorToken
    {
        public override int Priority => 2;
        public override float Calculate(float value1, float value2) => value1 * value2;
    }

    public class DivideToken : BinaryOperatorToken
    {
        public override int Priority => 2;
        public override float Calculate(float value1, float value2) => value1 / value2;
    }

    public class UnaryMinusToken : UnaryOperatorToken
    {
        public override int Priority => 3;
        public override float Calculate(float value) => -value;
    }

    public class SinToken : UnaryOperatorToken
    {
        public override int Priority => 3;
        public override float Calculate(float value) => Mathf.Sin(value * Mathf.PI);
    }
}