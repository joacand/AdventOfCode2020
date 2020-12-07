using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day7 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay7.txt");

            var input = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(input);
            var part2result = SolvePart2(input);

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(IEnumerable<string> input)
        {
            var bags = ParseAndConnectBags(input);
            var shinyGoldenBag = bags.First(x => x.Id.Equals("shiny gold"));

            return shinyGoldenBag.CountContainedByBags(new() { shinyGoldenBag }).ToString();
        }

        private static string SolvePart2(IEnumerable<string> input)
        {
            var bags = ParseAndConnectBags(input);
            var shinyGoldenBag = bags.First(x => x.Id.Equals("shiny gold"));

            // Subtract one since the shiny gold bag itself should not be counted
            return (shinyGoldenBag.CountTotalContainBags() - 1).ToString();
        }

        private static List<Bag> ParseAndConnectBags(IEnumerable<string> input)
        {
            var bags = input.Select(x => new Bag(x.Split("contain")[0].CleanBagId())).ToList();

            ConnectBags(bags, input);

            return bags;
        }

        private static void ConnectBags(IEnumerable<Bag> bags, IEnumerable<string> input)
        {
            foreach (var bagInput in input)
            {
                var containSplit = bagInput.Split("contain");
                var bag = bags.First(x => x.Id.Equals(containSplit[0].CleanBagId()));

                var connectedBags = containSplit[1].Contains("no other bags", StringComparison.OrdinalIgnoreCase)
                    ? new List<(string bagId, int weight)>()
                    : containSplit[1]
                        .Split(',')
                        .Select(x => (
                            bagId: x.Remove(0, 2).Trim('.').CleanBagId(),
                            weight: int.Parse(x.Substring(0, 2).Trim())
                            ));

                foreach (var (bagId, weight) in connectedBags)
                {
                    var connectedBag = bags.First(x => x.Id.Equals(bagId));
                    bag.ContainBags.Add((connectedBag, weight));
                    connectedBag.IsContainedByBags.Add(bag);
                }
            }
        }

        public record Bag(string Id)
        {
            public List<(Bag bag, int weight)> ContainBags { get; } = new();
            public List<Bag> IsContainedByBags { get; } = new();

            // Part 1
            public int CountContainedByBags(List<Bag> BagsAccountedFor)
            {
                var count = BagsAccountedFor.Contains(this) ? 0 : 1;
                BagsAccountedFor.Add(this);
                foreach (var bag in IsContainedByBags)
                {
                    count += bag.CountContainedByBags(BagsAccountedFor);
                }
                return count;
            }

            // Part 2
            public int CountTotalContainBags()
            {
                var count = 1;
                foreach (var (bag, weight) in ContainBags)
                {
                    count += weight * bag.CountTotalContainBags();
                }
                return count;
            }
        }
    }

    public static class StringExtensions
    {
        public static string CleanBagId(this string bagId)
        {
            return bagId
                .Replace("bags", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("bag", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Trim();
        }
    }
}
