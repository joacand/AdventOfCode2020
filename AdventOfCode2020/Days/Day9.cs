using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day9 : IDay
    {
        private const int PreambleSize = 25;

        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay9.txt");

            var input = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(input.Select(long.Parse).ToArray());
            var part2result = SolvePart2(input.Select(long.Parse).ToArray());

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(long[] input)
        {
            return FindInvalidNumber(input).ToString();
        }

        private static string SolvePart2(long[] input)
        {
            var invalidNumber = FindInvalidNumber(input);

            return FindContiguousNumber(invalidNumber, input).ToString();
        }

        private static long FindInvalidNumber(long[] input)
        {
            for (var i = 0; i < input[PreambleSize..].Length; i++)
            {
                var number = input[PreambleSize..][i];
                if (!IsValid(number, input[i..(i + PreambleSize)]))
                {
                    return number;
                }
            }

            throw new Exception("No answer found");
        }

        private static bool IsValid(long number, long[] numbersBefore)
        {
            return numbersBefore.Any(x => numbersBefore.Contains(Math.Abs(x - number)));
        }

        private static long FindContiguousNumber(long invalidNumber, long[] input)
        {
            LinkedList<long> contiguousNumbers = new();
            long accumulator = 0;

            foreach (var number in input)
            {
                contiguousNumbers.AddLast(number);
                accumulator += number;

                while (accumulator > invalidNumber)
                {
                    accumulator -= contiguousNumbers.First.Value;
                    contiguousNumbers.RemoveFirst();
                }

                if (accumulator == invalidNumber)
                {
                    return contiguousNumbers.Min() + contiguousNumbers.Max();
                }
            }

            throw new Exception("No answer found");
        }
    }
}
