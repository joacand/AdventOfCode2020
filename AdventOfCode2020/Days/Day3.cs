using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day3 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay3.txt");

            var input = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(input);
            var part2result = SolvePart2(input);

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<string> input)
        {
            var horizontalLength = input.First().Length;

            var treeCount = CalculateTrees(input, 3, 1, horizontalLength);

            return treeCount.ToString();
        }

        private static string SolvePart2(List<string> input)
        {
            var horizontalLength = input.First().Length;

            var productTrees = (long)
                CalculateTrees(input, 1, 1, horizontalLength) *
                CalculateTrees(input, 3, 1, horizontalLength) *
                CalculateTrees(input, 5, 1, horizontalLength) *
                CalculateTrees(input, 7, 1, horizontalLength) *
                CalculateTrees(input, 1, 2, horizontalLength);

            return productTrees.ToString();
        }

        private static int CalculateTrees(List<string> input, int rightMove, int downMove, int horizontalLength)
        {
            var treeCount = 0;
            for (int i = 0, pos = 0; i < input.Count; i += downMove, pos = (pos + rightMove) % horizontalLength)
            {
                if (input[i][pos] == '#')
                {
                    treeCount++;
                }
            }
            return treeCount;
        }
    }
}
