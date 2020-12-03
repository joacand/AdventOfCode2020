using AdventOfCode2020.Data;

namespace AdventOfCode2020.Days
{
    public class Day3 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay3.txt");

            var part1result = SolvePart1();
            var part2result = SolvePart2();

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1()
        {
            return string.Empty;
        }

        private static string SolvePart2()
        {
            return string.Empty;
        }
    }
}
