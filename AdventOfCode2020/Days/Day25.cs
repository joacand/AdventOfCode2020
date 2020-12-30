using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day25 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay25.txt");

            var lines = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(lines.Select(long.Parse).ToList());
            var part2result = SolvePart2(lines.Select(long.Parse).ToList());

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<long> input)
        {
            var publicKeyCard = input[0];
            var publicKeyDoor = input[1];

            var loopSizeCard = EncryptionKeyFinder.FindLoopSize(publicKeyCard);
            var encryptionKey = EncryptionKeyFinder.CalculateEncrypionKey(loopSizeCard, publicKeyDoor);

            return encryptionKey.ToString();
        }

        private static string SolvePart2(List<long> input)
        {
            return "No part 2 in this puzzle";
        }

        private static class EncryptionKeyFinder
        {
            public static long FindLoopSize(long publicKey, long subjectNumber = 7)
            {
                var loopsize = 0;

                long val = 1;
                while (val != publicKey)
                {
                    loopsize++;

                    val *= subjectNumber;
                    val %= 20201227;
                }

                return loopsize;
            }

            public static long CalculateEncrypionKey(long loopSize, long subjectNumber)
            {
                long val = 1;
                for (var i = 0; i < loopSize; i++)
                {
                    val *= subjectNumber;
                    val %= 20201227;
                }
                return val;
            }
        }
    }
}
