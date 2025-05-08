using System;
using System.Collections.Generic;

namespace FMLib.ExpressionParser
{
    public class ParseToPostfix
    {
        private readonly List<IToken> tokens;

        private List<IToken> postfix;
        private Stack<IToken> opStack;
        private IToken prevToken;


        public ParseToPostfix(List<IToken> tokens)
        {
            this.tokens = tokens;
        }

        public List<IToken> Parse()
        {
            postfix = new List<IToken>();
            opStack = new Stack<IToken>();
            prevToken = null;

            foreach (var token in tokens)
            {
                ProcessToken(token);
                prevToken = token;
            }

            while (opStack.Count > 0)
            {
                postfix.Add(opStack.Pop());
            }

            return postfix;
        }

        private void ProcessToken(IToken token)
        {
            switch (token)
            {
                case OperandToken operand:
                    ProcessOperand(operand);
                    break;
                case BracketToken bracket:
                    ProcessBracket(bracket);
                    break;
                case OperatorToken op:
                    ProcessOperator(op);
                    break;
            }
        }

        private void ProcessOperand(IToken token)
        {
            postfix.Add(token);
        }

        private void ProcessBracket(BracketToken bracket)
        {
            if (bracket.IsOpen)
            {
                opStack.Push(bracket);
            }
            else
            {
                var find = false;
                while (opStack.Count > 0)
                {
                    var cur = opStack.Pop();
                    if (cur is BracketToken)
                    {
                        find = true;
                        break;
                    }
                    else
                    {
                        postfix.Add(cur);
                    }
                }
                if (!find)
                    throw new ArgumentException("bracket error");
            }
        }

        private void ProcessOperator(OperatorToken op)
        {
            var tokenOp = op;
            if (op is MinusToken && !(prevToken is OperandToken))
            {
                tokenOp = new UnaryMinusToken();
            }

            while (opStack.Count > 0)
            {
                var top = opStack.Peek();
                if (top is OperatorToken topOp)
                {
                    if (topOp.Priority >= tokenOp.Priority)
                    {
                        opStack.Pop();
                        postfix.Add(topOp);
                        continue;
                    }
                }

                break;
            }
            opStack.Push(tokenOp);
        }
    }
}