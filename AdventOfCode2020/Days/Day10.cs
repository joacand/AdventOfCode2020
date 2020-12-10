using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day10 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay10.txt");

            var input = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(input.Select(x => int.Parse(x)).ToList());
            var part2result = SolvePart2(input.Select(x => int.Parse(x)).ToList());

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<int> input)
        {
            input.Sort();

            return CalculateDifferences(input).ToString();
        }

        private static string SolvePart2(List<int> input)
        {
            input.Sort();

            return CalculatePermutations(input, 0, input.Max(), new()).ToString();
        }

        private static int CalculateDifferences(List<int> input)
        {
            var oneDiff = 1;
            var twoDiff = 0;
            var threeDiff = 1;

            for (int i = 0; i + 1 < input.Count; i++)
            {
                switch (input[i + 1] - input[i])
                {
                    case 1:
                        oneDiff++;
                        break;
                    case 2:
                        twoDiff++;
                        break;
                    case 3:
                        threeDiff++;
                        break;
                }
            }
            return oneDiff * threeDiff;
        }

        private static long CalculatePermutations(List<int> input, int joleValueToTest, int deviceJoles, Dictionary<int, long> memoizations)
        {
            if (memoizations.ContainsKey(joleValueToTest))
            {
                return memoizations[joleValueToTest];
            }

            if (joleValueToTest == deviceJoles)
            {
                return 1;
            }

            long result = 0;

            if (input.Contains(joleValueToTest + 1))
            {
                result += CalculatePermutations(input, joleValueToTest + 1, deviceJoles, memoizations);
            }
            if (input.Contains(joleValueToTest + 2))
            {
                result += CalculatePermutations(input, joleValueToTest + 2, deviceJoles, memoizations);
            }
            if (input.Contains(joleValueToTest + 3))
            {
                result += CalculatePermutations(input, joleValueToTest + 3, deviceJoles, memoizations);
            }

            memoizations.Add(joleValueToTest, result);

            return result;
        }
    }
}
