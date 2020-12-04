using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2020.Days
{
    public class Day4 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay4.txt");

            var input = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(input);
            var part2result = SolvePart2(input);

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<string> input)
        {
            var passports = ParsePassports(input);

            var validator = new PassportValidator(
                new string[] { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" },
                new string[] { "cid" });

            return passports.Count(x => validator.IsValid(x)).ToString();
        }

        private static string SolvePart2(List<string> input)
        {
            var passports = ParsePassports(input);

            var validationRules = CreateValidationRules();

            var validator = new PassportValidator(
                new string[] { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" },
                new string[] { "cid" },
                validationRules);

            return passports.Count(x => validator.IsValid(x)).ToString();
        }

        private static List<Passport> ParsePassports(List<string> input)
        {
            List<Passport> passports = new();

            Passport passportBeingProcessed = new();
            foreach (var i in input)
            {
                if (string.IsNullOrWhiteSpace(i))
                {
                    passports.Add(passportBeingProcessed.Build());
                    passportBeingProcessed = new();
                }
                else
                {
                    passportBeingProcessed.AddPart(i);
                }
            }
            // Last entry is not accounted for in above loop
            passports.Add(passportBeingProcessed.Build());

            return passports;
        }

        private static Dictionary<string, Func<string, bool>> CreateValidationRules()
        {
            /* Validation rules:
                byr (Birth Year) - four digits; at least 1920 and at most 2002.
                iyr (Issue Year) - four digits; at least 2010 and at most 2020.
                eyr (Expiration Year) - four digits; at least 2020 and at most 2030.
                hgt (Height) - a number followed by either cm or in:
                    If cm, the number must be at least 150 and at most 193.
                    If in, the number must be at least 59 and at most 76.
                hcl (Hair Color) - a # followed by exactly six characters 0-9 or a-f.
                ecl (Eye Color) - exactly one of: amb blu brn gry grn hzl oth.
                pid (Passport ID) - a nine-digit number, including leading zeroes.
                cid (Country ID) - ignored, missing or not.
            */
            var validationRules = new Dictionary<string, Func<string, bool>>
            {
                { "byr", x => { return int.TryParse(x, out int birthYear) && birthYear >= 1920 && birthYear <= 2002; } },
                { "iyr", x => { return int.TryParse(x, out int issueYear) && issueYear >= 2010 && issueYear <= 2020; } },
                { "eyr", x => { return int.TryParse(x, out int expirationYear) && expirationYear >= 2020 && expirationYear <= 2030; } },
                {
                    "hgt", x =>
                    {
                        if (x.Contains("cm"))
                        {
                            x = x.Replace("cm", string.Empty);
                            return int.TryParse(x, out int height) && height >= 150 && height <= 193;
                        }
                        if (x.Contains("in"))
                        {
                            x = x.Replace("in", string.Empty);
                            return int.TryParse(x, out int height) && height >= 59 && height <= 76;
                        }
                        return false;
                    }
                },
                { "hcl", x => { return x.First().Equals('#') && x.Skip(1).Count() == 6 && x.Skip(1).All(c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f')); } },
                { "ecl", x => { return (new string[] { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" }).Count(color => color.Equals(x)) == 1; } },
                { "pid", x => { return int.TryParse(x, out int _) && x.Length == 9; } }
            };

            return validationRules;
        }
    }

    public record PassportValidator(string[] RequiredFields, string[] OptionalFields, Dictionary<string, Func<string, bool>> ValidationRules = null)
    {
        public bool IsValid(Passport passport)
        {
            return
                ContainsRequiredFields(passport) &&
                FollowsRules(passport);
        }

        public bool ContainsRequiredFields(Passport passport)
        {
            foreach (var field in RequiredFields)
            {
                if (!passport.FieldExists(field))
                {
                    return false;
                }
            }
            return true;
        }

        public bool FollowsRules(Passport passport)
        {
            if (ValidationRules == null)
            {
                return true;
            }
            foreach (var rule in ValidationRules)
            {
                var fieldValue = passport.Fields[rule.Key];
                if (!rule.Value(fieldValue))
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class Passport
    {
        public Dictionary<string, string> Fields { get; set; } = new();

        private readonly StringBuilder rawPassport = new();

        public Passport AddPart(string passportPart)
        {
            rawPassport.Append($" {passportPart}");
            return this;
        }

        public Passport Build()
        {
            ParsePassport(rawPassport.ToString());
            return this;
        }

        public bool FieldExists(string field)
        {
            return Fields.ContainsKey(field);
        }

        private void ParsePassport(string rawPassport)
        {
            var cleaned = rawPassport.Replace("\r\n", " ");
            var fields = cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var field in fields)
            {
                var fieldValue = field.Split(':');
                Fields.Add(fieldValue[0], fieldValue[1]);
            }
        }
    }
}
