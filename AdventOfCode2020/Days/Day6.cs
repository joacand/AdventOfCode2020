using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day6 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay6.txt");

            var input = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(input);
            var part2result = SolvePart2(input);

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<string> answers)
        {
            return ParseGroups(answers).Sum(x => x.DistinctAnswers()).ToString();
        }

        private static string SolvePart2(List<string> answers)
        {
            return ParseGroups(answers).Sum(x => x.QuestionsEveryoneAnsweredYes()).ToString();
        }

        private static List<Group> ParseGroups(List<string> input)
        {
            List<Group> groups = new();

            Group groupBeingProcessed = new();
            foreach (var personAnswers in input)
            {
                if (string.IsNullOrWhiteSpace(personAnswers))
                {
                    groups.Add(groupBeingProcessed);
                    groupBeingProcessed = new();
                }
                else
                {
                    groupBeingProcessed.AddPerson(personAnswers);
                }
            }
            // Last entry is not accounted for in above loop
            groups.Add(groupBeingProcessed);

            return groups;
        }

        private class Group
        {
            public List<Person> Persons { get; } = new();

            public int DistinctAnswers()
            {
                return Persons.SelectMany(x => x.Answers).Distinct().Count();
            }

            public int QuestionsEveryoneAnsweredYes()
            {
                var answerCount = 0;
                for (char c = 'a'; c <= 'z'; c++)
                {
                    if (Persons.All(x => x.Answers.Contains(c, StringComparison.OrdinalIgnoreCase)))
                    {
                        answerCount++;
                    }
                }
                return answerCount;
            }

            public void AddPerson(string answers)
            {
                Persons.Add(new Person(answers));
            }
        }

        private record Person(string Answers);
    }
}
