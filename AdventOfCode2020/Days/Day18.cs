using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day18 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay18.txt");

            var lines = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(lines.ToList());
            var part2result = SolvePart2(lines.ToList());

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<string> input)
        {
            var evaluator = new Evaluator();

            long result = 0;
            foreach (var expr in input)
            {
                result += evaluator.EvaluateExpression(expr, false);
            }
            return result.ToString();
        }

        private static string SolvePart2(List<string> input)
        {
            var evaluator = new Evaluator();

            long result = 0;
            foreach (var expr in input)
            {
                result += evaluator.EvaluateExpression(expr, true);
            }
            return result.ToString();
        }

        private class Evaluator
        {
            private Lexer lexer;
            private Token token;

            public long EvaluateExpression(string expr, bool usePrecedence)
            {
                lexer = new Lexer(expr);

                var rpnNumber = CreateReversePolishNotation(usePrecedence);

                return EvaluateRPN(rpnNumber);
            }

            /// <summary>
            /// Shunting-yard algortihm:
            /// https://en.wikipedia.org/wiki/Shunting-yard_algorithm
            /// </summary>
            private Queue<Token> CreateReversePolishNotation(bool usePrecedence)
            {
                Queue<Token> outputQueue = new();
                Stack<Token> operatorStack = new();

                while ((token = lexer.GetToken()) != null)
                {
                    switch (token.Kind)
                    {
                        case TokenKind.Number:
                            {
                                outputQueue.Enqueue(token);
                                break;
                            }
                        case TokenKind.Operator:
                            {
                                // + has higher precedence than * in this scenario
                                while (operatorStack.TryPeek(out Token r) && r.Value != "(" && (r.Value == "+" || !usePrecedence))
                                {
                                    outputQueue.Enqueue(operatorStack.Pop());
                                }
                                operatorStack.Push(token);
                                break;
                            }
                        case TokenKind.Deliminater when token.Value == "(":
                            {
                                operatorStack.Push(token);
                                break;
                            }
                        case TokenKind.Deliminater when token.Value == ")":
                            {
                                while (operatorStack.TryPeek(out Token r) && r.Value != "(")
                                {
                                    outputQueue.Enqueue(operatorStack.Pop());
                                }
                                if (operatorStack.TryPeek(out Token rr) && rr.Value == "(")
                                {
                                    operatorStack.Pop();
                                }
                                break;
                            }
                    }
                }
                while (operatorStack.Any())
                {
                    outputQueue.Enqueue(operatorStack.Pop());
                }

                return outputQueue;
            }

            private static long EvaluateRPN(Queue<Token> reversePolishNotationTokens)
            {
                Stack<long> result = new();

                foreach (var token in reversePolishNotationTokens)
                {
                    if (token.Kind == TokenKind.Number)
                    {
                        result.Push(long.Parse(token.Value));
                    }
                    else
                    {
                        var y = result.Pop();
                        var x = result.Pop();
                        switch (token.Value)
                        {
                            case "+":
                                result.Push(x + y);
                                break;
                            case "*":
                                result.Push(x * y);
                                break;
                            default:
                                throw new Exception($"Invalid operator: {token.Value}");
                        }
                    }
                }

                return result.Pop();
            }
        }

        private class Lexer
        {
            private readonly List<Token> tokens = new();
            private int currentIndex = 0;
            private string beingParsed = "";
            private char currentChar = '0';
            private int charIndex = 0;

            public Lexer(string input)
            {
                GenerateAllTokens(input);
            }

            public Token GetToken()
            {
                if (currentIndex == tokens.Count)
                {
                    return null;
                }
                return tokens[currentIndex++];
            }

            private void GenerateAllTokens(string input)
            {
                beingParsed = input;
                currentChar = '0';
                charIndex = -1;

                Token token;
                while ((token = GenerateToken()) != null)
                {
                    if (token != null)
                        tokens.Add(token);
                }
            }

            public Token GenerateToken()
            {
                Token token = new();
                currentChar = GetNextChar();

                while (currentChar != '\0')
                {
                    if (char.IsWhiteSpace(currentChar))
                    {
                        currentChar = GetNextChar();
                        continue;
                    }
                    if (char.IsDigit(currentChar))
                    {
                        var alpha = "";
                        var previndex = charIndex;
                        while (char.IsDigit(currentChar))
                        {
                            previndex = charIndex;
                            alpha += currentChar;
                            currentChar = GetNextChar();
                        }
                        charIndex = previndex;
                        token.Value = alpha;
                        token.Kind = TokenKind.Number;
                        return token;
                    }
                    else if (currentChar == '+' || currentChar == '-' || currentChar == '*')
                    {
                        token.Kind = TokenKind.Operator;
                        token.Value = currentChar.ToString();
                        return token;
                    }
                    else if (currentChar == '(' || currentChar == ')')
                    {
                        token.Kind = TokenKind.Deliminater;
                        token.Value = currentChar.ToString();
                        return token;
                    }
                }
                return null;
            }

            private char GetNextChar()
            {
                charIndex++;
                return charIndex >= beingParsed.Length
                    ? '\0'
                    : beingParsed[charIndex];
            }
        }

        private class Token
        {
            public TokenKind Kind { get; set; }
            public string Value { get; set; }
        }

        private enum TokenKind
        {
            Empty,
            Operator,
            Number,
            Deliminater
        }
    }
}
