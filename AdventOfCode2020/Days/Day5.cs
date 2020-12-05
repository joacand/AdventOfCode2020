using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day5 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay5.txt");

            var input = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(input);
            var part2result = SolvePart2(input);

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<string> boardingPasses)
        {
            return boardingPasses.Select(x => CalculateSeatId(x)).Max().ToString();
        }

        private static string SolvePart2(List<string> boardingPasses)
        {
            var seatIds = boardingPasses.Select(x => CalculateSeatId(x)).OrderBy(x => x).ToList();

            return FindMissingIncrementalSeat(seatIds).ToString();
        }

        private static int FindMissingIncrementalSeat(List<int> seatIds)
        {
            var comparisonSeatId = seatIds.First();

            return seatIds.First(x => x != comparisonSeatId++) - 1;
        }

        private static int CalculateSeatId(string boardingPass)
        {
            (int row, int column) = CalculateSeatPosition(boardingPass);
            return row * 8 + column;
        }

        private static (int row, int column) CalculateSeatPosition(string boardingPass)
        {
            var row = CalculatePosition(
                new() { lower = 0, upper = 127 },
                boardingPass.Replace("R", string.Empty).Replace("L", string.Empty));

            var column = CalculatePosition(
                new() { lower = 0, upper = 7 },
                boardingPass.Replace("F", string.Empty).Replace("B", string.Empty));

            return (row, column);
        }

        private static int CalculatePosition((int lower, int upper) range, string operations)
        {
            foreach (var operation in operations)
            {
                range = CalculateNewBound(range, operation);
                if (range.lower == range.upper)
                {
                    return range.lower;
                }
            }
            throw new Exception("No answer found");
        }

        private static (int lower, int upper) CalculateNewBound((int lower, int upper) range, char operation)
        {
            return operation switch
            {
                'F' => (range.lower, range.upper - ((range.upper - range.lower) / 2) - 1),
                'B' => (range.lower + ((range.upper - range.lower) / 2) + 1, range.upper),
                'L' => (range.lower, range.upper - ((range.upper - range.lower) / 2) - 1),
                'R' => (range.lower + ((range.upper - range.lower) / 2) + 1, range.upper),
                _ => throw new ArgumentException($"Invalid operation {operation}", nameof(operation)),
            };
        }
    }
}
