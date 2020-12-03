using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day2 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay2.txt");

            var lines = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var entries = lines.Select(x => new Entry
            {
                Policy = x.Split(':')[0].Trim(),
                Password = x.Split(':')[1].Trim()
            }).ToList();

            var part1result = SolvePart1(entries);
            var part2result = SolvePart2(entries);

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<Entry> entries)
        {
            return entries.Count(x => x.ValidAccordingToOldPolicy()).ToString();
        }

        private static string SolvePart2(List<Entry> entries)
        {
            return entries.Count(x => x.ValidAccordingToNewPolicy()).ToString();
        }
    }

    public class Entry
    {
        public string Policy { get; set; }
        public string Password { get; set; }

        // Part 1 policy validator
        public bool ValidAccordingToOldPolicy()
        {
            (int minNumber, int maxNumber, char character) = ParsePolicy();

            var characterHits = Password.Count(c => c == character);

            return characterHits >= minNumber && characterHits <= maxNumber;
        }

        // Part 2 policy validator
        public bool ValidAccordingToNewPolicy()
        {
            (int firstPos, int secondPos, char character) = ParsePolicy();

            return Password[firstPos - 1] == character ^ Password[secondPos - 1] == character;
        }

        private (int firstNumber, int secondNumber, char character) ParsePolicy()
        {
            var tempSplit = Policy.Split('-')[1].Split(' ');

            var firstNumber = int.Parse(Policy.Split('-')[0]);
            var secondNumber = int.Parse(tempSplit[0]);
            var character = char.Parse(tempSplit[1]);

            return (firstNumber, secondNumber, character);
        }
    }
}
