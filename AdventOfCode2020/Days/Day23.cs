using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day23 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay23.txt");

            var part1result = SolvePart1(inputRaw);
            var part2result = SolvePart2(inputRaw);

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(string input)
        {
            return Game.PerformMoves(input, false);
        }

        private static string SolvePart2(string input)
        {
            return Game.PerformMoves(input, true);
        }

        private class Game
        {
            public static string PerformMoves(string cups, bool partTwo)
            {
                var cupListInt = partTwo
                    ? PadCupsToMillion(cups)
                    : cups.Select(x => (int)char.GetNumericValue(x)).ToList();

                var iterations = partTwo ? 10000000 : 100;

                var cupLinkedList = new LinkedList<int>(cupListInt);
                var currentCupNode = cupLinkedList.First;
                var max = cupListInt.Max();

                for (var i = 0; i < iterations; i++)
                {
                    (cupLinkedList, currentCupNode) = Move(cupLinkedList, currentCupNode, max);
                }

                if (!partTwo)
                {
                    var calcList = new List<int>(cupLinkedList.ToList());
                    var finalResult = "";
                    var onePosition = calcList.IndexOf(1);
                    for (var i = 0; i < calcList.Count - 1; i++)
                    {
                        finalResult += calcList[(i + 1 + onePosition) % calcList.Count];
                    }
                    return finalResult;
                }

                var oneNode = cupLinkedList.Find(1);
                var starOneNode = oneNode.Next ?? cupLinkedList.First;
                var starTwoNode = starOneNode.Next ?? cupLinkedList.First;
                var result = (long)starOneNode.Value * starTwoNode.Value;
                return result.ToString();
            }

            private static List<int> PadCupsToMillion(string cups)
            {
                var cupList = new List<int>(cups.Select(x => (int)char.GetNumericValue(x)).ToList());
                for (int i = cupList.Max(); i < 1000000; i++)
                {
                    cupList.Add(i + 1);
                }
                return cupList;
            }

            private static (LinkedList<int>, LinkedListNode<int>) Move(
                LinkedList<int> cups,
                LinkedListNode<int> currentCupNode,
                int max)
            {
                var destinationCupLabel = currentCupNode.Value - 1;

                var removedValues = cups.RemoveThreeValuesAfter(currentCupNode);

                while (removedValues.Contains(destinationCupLabel) || destinationCupLabel < 1 || destinationCupLabel > max)
                {
                    destinationCupLabel--;
                    if (destinationCupLabel < 1 || destinationCupLabel > max)
                    {
                        destinationCupLabel = max;
                    }
                }

                var destinationNode = cups.Find(destinationCupLabel);

                for (int i = 2; i >= 0; i--)
                {
                    cups.AddAfter(destinationNode, removedValues[i]);
                }

                return (cups, currentCupNode.Next ?? cups.First);
            }
        }

        private class LinkedList<T>
        {
            private Dictionary<T, LinkedListNode<T>> ValueToNodeMap { get; } = new();

            public LinkedListNode<T> First { get; private set; }

            public LinkedList(List<T> input)
            {
                First = new LinkedListNode<T> { Value = input[0], Next = null };
                ValueToNodeMap[input[0]] = First;

                var current = First;
                foreach (var val in input.Skip(1))
                {
                    var node = new LinkedListNode<T> { Value = val, Next = null };

                    ValueToNodeMap[val] = node;

                    current.Next = node;
                    current = node;
                }
            }

            public IEnumerable<T> ToList()
            {
                List<T> result = new();

                result.Add(First.Value);

                var current = First;
                while (current.Next != null)
                {
                    result.Add(current.Next.Value);
                    current = current.Next;
                }
                return result;
            }

            public LinkedListNode<T> Find(T val)
            {
                return ValueToNodeMap[val];
            }

            public void AddAfter(LinkedListNode<T> myLinkedListNode, T value)
            {
                var prevNode = myLinkedListNode.Next;
                var newNode = new LinkedListNode<T>() { Value = value, Next = prevNode };

                myLinkedListNode.Next = newNode;
                ValueToNodeMap[value] = newNode;
            }

            public T[] RemoveThreeValuesAfter(LinkedListNode<T> currentCupNode)
            {
                var loopAround = currentCupNode.Next?.Next?.Next == null;

                var n1 = currentCupNode.Next ?? First;
                var n2 = n1.Next ?? First;
                var n3 = n2.Next ?? First;

                T[] removedValues = new T[3] { n1.Value, n2.Value, n3.Value };

                if (!loopAround)
                {
                    currentCupNode.Next = n3.Next;
                    return removedValues;
                }

                if (currentCupNode.Next == null)
                {
                    First = First.Next.Next.Next;
                }
                else if (n1.Next == null)
                {
                    currentCupNode.Next = null;
                    First = First.Next.Next;
                }
                else if (n2.Next == null)
                {
                    currentCupNode.Next = null;
                    First = First.Next;
                }

                return removedValues;
            }
        }

        private class LinkedListNode<T>
        {
            public T Value { get; set; }
            public LinkedListNode<T> Next { get; set; }
        }
    }
}
