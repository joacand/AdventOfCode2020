using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day15 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay15.txt");

            var input = inputRaw.Split(',', StringSplitOptions.None).Select(x => int.Parse(x)).ToList();

            var part1result = SolvePart1(input.ToList());
            var part2result = SolvePart2(input.ToList());

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<int> input)
        {
            return DetermineNumberSpoken(input, 2020).ToString();
        }

        private static string SolvePart2(List<int> input)
        {
            return DetermineNumberSpoken(input, 30000000).ToString();
        }

        private static int DetermineNumberSpoken(List<int> input, int numbersSpoken)
        {
            Dictionary<int, (int latest, int secondLatest)> turnSpoken = new();
            var numberSpoken = 0;

            for (int i = 0; i < numbersSpoken; i++)
            {
                if (i < input.Count)
                {
                    numberSpoken = input[i];
                    turnSpoken[numberSpoken] = (i + 1, 0);
                    continue;
                }

                var (latest, secondLatest) = turnSpoken[numberSpoken];
                numberSpoken = secondLatest == 0 ? 0 : latest - secondLatest;

                turnSpoken[numberSpoken] = turnSpoken.ContainsKey(numberSpoken)
                    ? (i + 1, turnSpoken[numberSpoken].latest)
                    : (i + 1, 0);
            }

            return numberSpoken;
        }
    }
}
