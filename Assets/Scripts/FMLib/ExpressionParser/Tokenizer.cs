using System;
using System.Collections.Generic;

namespace FMLib.ExpressionParser
{
    public class Tokenizer
    {
        private readonly string expr;
        private int idx;

        public Tokenizer(string expression)
        {
            expr = expression;
        }

        public List<IToken> Tokenize()
        {
            idx = 0;
            var tokens = new List<IToken>();
            while (idx < expr.Length)
            {
                var cur = expr[idx];
                if (IsAlphabet(cur))
                {
                    tokens.Add(GetVariable());
                }
                else if (IsNumeric(cur))
                {
                    tokens.Add(GetValue());
                }
                else if (IsBracket(cur))
                {
                    tokens.Add(GetBracket());
                }
                else if (IsOperator(cur))
                {
                    tokens.Add(GetOperator());
                }
                else if (IsEmpty(cur))
                {
                    idx++;
                }
                else
                {
                    throw new ArgumentException($"unknown character : {cur}");
                }
            }
            return tokens;
        }

        private IToken GetVariable()
        {
            var curExpr = GetString(IsAlphabet);
            return new VariableToken(curExpr);
        }

        private IToken GetValue()
        {
            var curExpr = GetString(IsNumeric);
            if (float.TryParse(curExpr, out var result))
            {
                return new ValueToken(result);
            }
            throw new ArgumentException($"parse error : {curExpr}");
        }

        private IToken GetBracket()
        {
            var cur = expr[idx];
            idx++;
            return cur switch
            {
                '(' => new BracketToken(true),
                ')' => new BracketToken(false),
                _ => throw new ArgumentException($"unknown bracket : {cur}")
            };
        }

        private IToken GetOperator()
        {
            var cur = expr[idx];

            if (cur == '!')
            {
                idx++;
                var curExpr = GetString(IsAlphabet);
                return curExpr switch
                {
                    "sin" => new SinToken(),
                    _ => throw new ArgumentException($"unknown operator : !{curExpr}")
                };
            }
            else
            {
                idx++;
                return cur switch
                {
                    '+' => new PlusToken(),
                    '-' => new MinusToken(),
                    '*' => new MultiplyToken(),
                    '/' => new DivideToken(),
                    _ => throw new ArgumentException($"unknown operator : {cur}")
                };
            }
        }

        private string GetString(Func<char, bool> checker)
        {
            var startIdx = idx;
            while (checker(expr[idx]))
            {
                idx++;
                if (idx >= expr.Length)
                    break;
            }
            if (startIdx == idx)
                throw new ArgumentException("get string error");
            return expr.Substring(startIdx, idx - startIdx);
        }


        private bool IsAlphabet(char c)
        {
            return ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z');
        }

        private bool IsNumeric(char c)
        {
            return ('0' <= c && c <= '9') || c == '.';
        }

        private bool IsBracket(char c)
        {
            return c == '(' || c == ')';
        }

        private bool IsOperator(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '!';
        }

        private bool IsEmpty(char c)
        {
            return c == ' ' || c == '\t';
        }
    }
}