using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day16 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay16.txt");

            var lines = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(lines.ToList());
            var part2result = SolvePart2(lines.ToList());

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<string> input)
        {
            (List<Ticket> tickets, List<int> yourTicket, TicketValidator ticketValidator) = InputParser.ParseInput(input);

            return tickets.SelectMany(x => x.GetInvalidValues(ticketValidator)).Sum().ToString();
        }

        private static string SolvePart2(List<string> input)
        {
            (List<Ticket> tickets, List<int> yourTicket, TicketValidator ticketValidator) = InputParser.ParseInput(input);

            var validTickets = tickets.Where(ticket => !ticket.GetInvalidValues(ticketValidator).Any()).ToList();

            var ruleMapping = new TicketDecoder(validTickets, ticketValidator).GetRuleMapping();

            var departureIndices = ruleMapping.Where(x => x.Key.StartsWith("departure")).Select(x => x.Value).ToList();

            return departureIndices
                .Select(x => yourTicket[x])
                .Product()
                .ToString();
        }

        private class Ticket
        {
            public List<int> Values { get; set; } = new();

            public Ticket(string rawValues)
            {
                Values = rawValues.Split(',').Select(x => int.Parse(x)).ToList();
            }

            public IEnumerable<int> GetInvalidValues(TicketValidator ticketValidator)
            {
                return Values.Where(v => !ticketValidator.ValidValue(v));
            }
        }

        private class TicketValidator
        {
            private Dictionary<string, Rule> Rules { get; set; } = new();

            public TicketValidator(IEnumerable<string> rawRules)
            {
                foreach (var rawRule in rawRules)
                {
                    var split = rawRule.Split(':');
                    var ranges = split[1].Split("or");

                    var ruleName = split[0].Trim();
                    Rule rule = new();

                    foreach (var range in ranges)
                    {
                        var rangeValues = range.Split('-');
                        rule.ValidRanges.Add((int.Parse(rangeValues[0].Trim()), int.Parse(rangeValues[1].Trim())));
                    }

                    Rules.Add(ruleName, rule);
                }
            }

            public IEnumerable<string> FindValidRuleForValues(IEnumerable<int> values)
            {
                return Rules.Where(rule => values.All(value => rule.Value.ValidValue(value))).Select(rule => rule.Key);
            }

            public bool ValidValue(int value)
            {
                return Rules.Values.Any(rule => rule.ValidValue(value));
            }

            private record Rule()
            {
                public List<(int min, int max)> ValidRanges { get; set; } = new();

                public bool ValidValue(int value)
                {
                    return ValidRanges.Any(range => value >= range.min && value <= range.max);
                }
            }
        }

        private class TicketDecoder
        {
            private List<Ticket> Tickets { get; }
            private TicketValidator TicketValidator { get; }

            public TicketDecoder(List<Ticket> tickets, TicketValidator ticketValidator)
            {
                Tickets = tickets;
                TicketValidator = ticketValidator;
            }

            public Dictionary<string, int> GetRuleMapping()
            {
                Dictionary<string, int> ruleMapping = new();
                Dictionary<string, List<int>> intermediateRuleMapping = new();
                var ticketCount = Tickets[0].Values.Count;

                for (int i = 0; i < ticketCount; i++)
                {
                    var ruleNames = DetermineRulesForValues(Tickets.Select(x => x.Values[i]).ToList());

                    foreach (var ruleName in ruleNames)
                    {
                        if (!intermediateRuleMapping.ContainsKey(ruleName))
                        {
                            intermediateRuleMapping[ruleName] = new();
                        }
                        intermediateRuleMapping[ruleName].Add(i);
                    }
                }

                List<int> usedIndices = new();
                bool foundPerfectMatch;

                do
                {
                    foundPerfectMatch = false;

                    foreach (var ruleMap in intermediateRuleMapping)
                    {
                        var ruleMapWithoutUsedIndices = ruleMap.Value.Where(x => !usedIndices.Contains(x));

                        if (ruleMapWithoutUsedIndices.Count() == 1)
                        {
                            foundPerfectMatch = true;

                            ruleMapping.Add(ruleMap.Key, ruleMapWithoutUsedIndices.Single());
                            usedIndices.Add(ruleMapWithoutUsedIndices.Single());
                        }
                    }
                } while (foundPerfectMatch);

                return ruleMapping;
            }

            private IEnumerable<string> DetermineRulesForValues(IEnumerable<int> values)
            {
                return TicketValidator.FindValidRuleForValues(values);
            }
        }

        #region Parsing
        private static class InputParser
        {
            public static (List<Ticket> tickets, List<int> yourTicket, TicketValidator ticketValidator) ParseInput(List<string> input)
            {
                (List<string> rawRules, string yourTicketRaw, List<string> rawTickets) = SplitRulesAndTickets(input);

                var tickets = rawTickets.Select(x => new Ticket(x)).ToList();
                var yourTicket = yourTicketRaw.Split(',').Select(x => int.Parse(x)).ToList();
                var ticketValidator = new TicketValidator(rawRules);

                return (tickets, yourTicket, ticketValidator);
            }

            private static (List<string> rules, string yourTicket, List<string> tickets) SplitRulesAndTickets(List<string> input)
            {
                List<string> rules = new();
                string yourTicket = string.Empty;
                List<string> tickets = new();

                bool isRule = true;
                bool isYourTicket = false;

                foreach (var i in input)
                {
                    if (string.IsNullOrWhiteSpace(i))
                    {
                        if (isRule)
                        {
                            isRule = false; isYourTicket = true;
                        }
                        else if (isYourTicket)
                        {
                            isYourTicket = false;
                        }
                    }
                    else
                    {
                        if (isRule)
                        {
                            rules.Add(i);
                        }
                        else if (isYourTicket)
                        {
                            if (!i.Contains("your ticket"))
                                yourTicket = i;
                        }
                        else
                        {
                            if (!i.Contains("nearby tickets"))
                                tickets.Add(i);
                        }
                    }
                }
                return (rules, yourTicket, tickets);
            }
        }
        #endregion
    }

    public static class Day16Extensions
    {
        public static long Product(this IEnumerable<int> values)
        {
            long product = 1;
            foreach (var value in values)
            {
                product *= value;
            }
            return product;
        }
    }
}
