using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day8 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay8.txt");

            var input = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(input);
            var part2result = SolvePart2(input);

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(IEnumerable<string> input)
        {
            return new Computer(input).ExecuteUntilLoopOrTermination().accumulator;
        }

        private static string SolvePart2(IEnumerable<string> input)
        {
            return AccumulatorAfterModifiedTermination(input.ToList());
        }

        private static string AccumulatorAfterModifiedTermination(List<string> input)
        {
            for (int i = 0; i < input.Count; i++)
            {
                if (Instruction.Parse(input[i]).Operation.Equals("acc"))
                {
                    continue;
                }

                FlipJmpNop(input, i);

                var (accumulator, terminated) = new Computer(input).ExecuteUntilLoopOrTermination();
                if (terminated)
                {
                    return accumulator;
                }

                FlipJmpNop(input, i);
            }

            throw new Exception("No termination point found");
        }

        private static void FlipJmpNop(List<string> input, int position)
        {
            if (input[position].Contains("nop"))
            {
                input[position] = input[position].Replace("nop", "jmp");
            }
            else if (input[position].Contains("jmp"))
            {
                input[position] = input[position].Replace("jmp", "nop");
            }
        }

        private class Computer
        {
            private Instruction[] Program { get; init; }

            private int programCounter = 0;
            private int accumulator = 0;
            private List<int> visitedLocations = new();

            private bool IsLoop => visitedLocations.Contains(programCounter);
            private bool IsTermination => programCounter >= Program.Length;

            public Computer(IEnumerable<string> programInput)
            {
                Program = ParseProgram(programInput);
            }

            public (string accumulator, bool terminated) ExecuteUntilLoopOrTermination()
            {
                programCounter = 0;
                accumulator = 0;
                var loopDetected = false;
                var termination = false;
                visitedLocations = new();

                while (!loopDetected && !termination)
                {
                    ExecuteInstruction(Program[programCounter]);

                    loopDetected = IsLoop;
                    termination = IsTermination;

                    visitedLocations.Add(programCounter);
                }

                return (accumulator.ToString(), termination);
            }

            private void ExecuteInstruction(Instruction instruction)
            {
                switch (instruction.Operation)
                {
                    case "acc":
                        {
                            _ = instruction.Addition
                                ? accumulator += instruction.Argument
                                : accumulator -= instruction.Argument;
                            break;
                        }
                    case "jmp":
                        {
                            _ = instruction.Addition
                                ? programCounter += instruction.Argument
                                : programCounter -= instruction.Argument;
                            return;
                        }
                    case "nop":
                        break;
                    default:
                        throw new ArgumentException($"Unknown operation {instruction.Operation}");
                }
                programCounter++;
            }

            private static Instruction[] ParseProgram(IEnumerable<string> programInput)
            {
                return programInput.Select(instruction => Instruction.Parse(instruction)).ToArray();
            }
        }

        private record Instruction(string Operation, int Argument, bool Addition)
        {
            public static Instruction Parse(string instruction)
            {
                var parts = instruction.Split(' ');

                return new Instruction(
                    Operation: parts[0],
                    Argument: int.Parse(parts[1][1..]),
                    Addition: parts[1].First() == '+');
            }
        }
    }
}
