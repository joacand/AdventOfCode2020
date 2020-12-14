using AdventOfCode2020.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2020.Days
{
    public class Day14 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay14.txt");

            var input = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(input.ToList());
            var part2result = SolvePart2(input.ToList());

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<string> input)
        {
            var system = new ComputerSystem(new DecoderShipPart1());
            system.ExecuteProgram(input);

            return system.SumOfAddressValues().ToString();
        }

        private static string SolvePart2(List<string> input)
        {
            var system = new ComputerSystem(new DecoderShipPart2());
            system.ExecuteProgram(input);

            return system.SumOfAddressValues().ToString();
        }

        private class ComputerSystem
        {
            private IDecoderChip DecoderChip { get; }
            private Dictionary<long, long> AddressSpace { get; } = new();

            public ComputerSystem(IDecoderChip decoderChip)
            {
                DecoderChip = decoderChip;
            }

            public void ExecuteProgram(List<string> program)
            {
                var mask = string.Empty;

                foreach (var instruction in program)
                {
                    if (instruction.Contains("mask"))
                    {
                        mask = instruction.Split('=')[1].Trim();
                    }
                    else
                    {
                        DecoderChip.CalculateAndSetValue(instruction, mask, AddressSpace);
                    }
                }
            }

            public long SumOfAddressValues()
            {
                return AddressSpace.Values.Sum();
            }
        }

        public interface IDecoderChip
        {
            void CalculateAndSetValue(string addressAndValue, string mask, Dictionary<long, long> AddressSpace);
        }

        private class DecoderShipPart1 : IDecoderChip
        {
            public void CalculateAndSetValue(string addressAndValue, string mask, Dictionary<long, long> AddressSpace)
            {
                var address = long.Parse(addressAndValue.Split('[')[1].Split(']')[0]);
                var value = long.Parse(addressAndValue.Split('=')[1].Trim());

                AddressSpace[address] = ApplyMixedMask(value.ToBitArray(), mask).ToLong();
            }

            private static BitArray ApplyMixedMask(BitArray source, string mask)
            {
                var andMaskArray = mask.Replace('X', '1').ToBitArray();
                var orMaskArray = mask.Replace('X', '0').ToBitArray();

                source = source.And(andMaskArray);
                source = source.Or(orMaskArray);

                return source;
            }
        }

        private class DecoderShipPart2 : IDecoderChip
        {
            public void CalculateAndSetValue(string addressAndValue, string mask, Dictionary<long, long> AddressSpace)
            {
                var initialAddress = long.Parse(addressAndValue.Split('[')[1].Split(']')[0]);
                var value = long.Parse(addressAndValue.Split('=')[1].Trim());

                var addressesToSet = GetAddressesToSet(initialAddress.ToBitArray(), mask);

                foreach (var address in addressesToSet)
                {
                    AddressSpace[address] = value;
                }
            }

            private static List<long> GetAddressesToSet(BitArray initialAddress, string mask)
            {
                var orMaskArray = mask.Replace('X', '0').ToBitArray();
                var initialAddressAfterOr = orMaskArray.Or(initialAddress);

                return GetAddressesAfterFloatingMask(initialAddressAfterOr, mask);
            }

            private static List<long> GetAddressesAfterFloatingMask(BitArray initialAddress, string mask)
            {
                List<long> result = new();
                var floatingCount = mask.Count(c => c == 'X');

                for (var i = 0; i < 1 << floatingCount; i++)
                {
                    var binaryString = Convert.ToString(i, 2).PadLeft(floatingCount, '0');
                    var addressBuilder = initialAddress.ToStringBuilder();
                    int floatingsFound = 0;

                    for (var x = mask.Length - 1; x >= 0; x--)
                    {
                        if (mask[x] == 'X')
                        {
                            addressBuilder[x] = binaryString[floatingsFound++];
                        }
                    }

                    var address = addressBuilder.ToString().ToBitArray().ToLong();
                    result.Add(address);
                }

                return result;
            }
        }
    }

    public static class Day14Extensions
    {
        public static BitArray ToBitArray(this string src)
        {
            return new BitArray(src.Select(c => c == '1').ToArray());
        }

        public static StringBuilder ToStringBuilder(this BitArray src)
        {
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < src.Count; i++)
            {
                char c = src[i] ? '1' : '0';
                stringBuilder.Append(c);
            }

            return stringBuilder;
        }

        public static BitArray ToBitArray(this long src)
        {
            return Convert.ToString(src, 2).PadLeft(36, '0').ToBitArray();
        }

        public static long ToLong(this BitArray src)
        {
            var reversedSrc = src.Reverse();

            long result = 0;
            for (int i = 0; i < reversedSrc.Length; i++)
            {
                if (reversedSrc.Get(i))
                {
                    result |= 1L << i;
                }
            }
            return result;
        }

        public static BitArray Reverse(this BitArray array)
        {
            int length = array.Length;
            int mid = length / 2;

            for (int i = 0; i < mid; i++)
            {
                bool bit = array[i];
                array[i] = array[length - i - 1];
                array[length - i - 1] = bit;
            }
            return array;
        }
    }
}
