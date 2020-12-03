using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day1 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay1.txt");

            var input = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).Select(x => int.Parse(x)).ToList();

            var part1result = SolvePart1(input);
            var part2result = SolvePart2(input);

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<int> input)
        {
            input.Sort();

            for (int upper = input.Count - 1, lower = 0, sum = 0;
                upper >= 0; sum = input[upper] + input[lower])
            {
                if (sum == 2020)
                {
                    return (input[upper] * input[lower]).ToString();
                }
                else if (sum < 2020)
                {
                    lower++;
                }
                else if (sum > 2020)
                {
                    lower = lower - 1 < 0 ? 0 : lower - 1;
                    upper--;
                }
            }

            throw new Exception("No result found");
        }

        private static string SolvePart2(List<int> input)
        {
            input.Sort();

            for (int upper = input.Count - 1, lower = 0, lowerSnd = 1, sum = 0;
                upper >= 0; sum = input[upper] + input[lower] + input[lowerSnd])
            {
                if (sum == 2020)
                {
                    return (input[upper] * input[lower] * input[lowerSnd]).ToString();
                }
                else if (sum < 2020)
                {
                    lowerSnd++;
                }
                else if (sum > 2020 && lowerSnd > 1)
                {
                    lowerSnd = 1;
                    lower++;
                }
                else
                {
                    upper--;
                    lowerSnd = 1;
                    lower = 0;
                }
            }

            throw new Exception("No result found");
        }
    }
}
