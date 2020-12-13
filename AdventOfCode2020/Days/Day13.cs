using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day13 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay13.txt");

            var input = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
            var timestamp = int.Parse(input[0]);
            var busses = input[1].Split(',', StringSplitOptions.RemoveEmptyEntries);

            var part1result = SolvePart1(timestamp, busses.Where(x => x != "x").Select(int.Parse).ToList());
            var part2result = SolvePart2(busses.Select(x => x.Replace("x", "-1")).Select(int.Parse).ToList());

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(int timestamp, List<int> busses)
        {
            var shortestTime = int.MaxValue;
            var quickestBus = -1;

            foreach (var busId in busses)
            {
                var timeDiff = busId * ((timestamp - 1) / busId + 1) - timestamp;

                if (timeDiff < shortestTime)
                {
                    shortestTime = timeDiff;
                    quickestBus = busId;
                }
            }

            return (quickestBus * shortestTime).ToString();
        }

        private static string SolvePart2(List<int> busses)
        {
            var bussedWithIndex = new List<(int value, int offset)>();
            for (var i = 0; i < busses.Count; i++)
            {
                if (busses[i] > 0)
                {
                    bussedWithIndex.Add((busses[i], i));
                }
            }

            return FindSolution(bussedWithIndex).ToString();
        }

        private static long FindSolution(List<(int value, int offset)> bussesWithIndex)
        {
            long timestamp = bussesWithIndex[0].value;

            for (var i = 0; i < bussesWithIndex.Count; i++)
            {
                var solutionsToFind = bussesWithIndex.Take(i + 1).ToList();

                long iterationJump = 1;
                foreach (var (value, _) in solutionsToFind.Take(i))
                {
                    iterationJump *= value;
                }

                timestamp = FindTimestamp(solutionsToFind, timestamp, iterationJump);
            }

            return timestamp;
        }

        private static long FindTimestamp(List<(int value, int offset)> solutionsToFind, long timestamp, long iterationJump)
        {
            while (true)
            {
                if (TestSolution(timestamp, solutionsToFind))
                {
                    return timestamp;
                }
                timestamp += iterationJump;
            }
        }

        private static bool TestSolution(long timestamp, List<(int value, int offset)> solutionsToFind)
        {
            foreach (var (value, offset) in solutionsToFind)
            {
                if ((timestamp + offset) % value != 0)
                    return false;
            }
            return true;
        }
    }
}
