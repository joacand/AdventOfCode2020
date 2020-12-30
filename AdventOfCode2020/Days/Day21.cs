using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day21 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay21.txt");

            var lines = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(lines.ToList());
            var part2result = SolvePart2(lines.ToList());

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<string> input)
        {
            (var ingredientList, var allergenMapping) = ParseInput(input);
            (var ingredientsWithAllergens, _) = CalculateAllergens(allergenMapping);

            var ingredientsWithoutAllergens = allergenMapping.Values
                .SelectMany(x => x).SelectMany(x => x)
                .Distinct()
                .Except(ingredientsWithAllergens)
                .ToList();

            return ingredientList.Count(x => ingredientsWithoutAllergens.Contains(x)).ToString();
        }

        private static string SolvePart2(List<string> input)
        {
            (_, var allergenMapping) = ParseInput(input);
            (_, var ingredientAllergenMap) = CalculateAllergens(allergenMapping);

            return string.Join(',', ingredientAllergenMap.OrderBy(x => x.Key).Select(x => x.Value));
        }

        private static (List<string>, Dictionary<string, List<IEnumerable<string>>>) ParseInput(List<string> input)
        {
            Dictionary<string, List<IEnumerable<string>>> allergenMapping = new();
            List<string> ingredientList = new();

            foreach (var line in input)
            {
                var ingredientsAllergens = line.Split("(contains ");
                var ingredients = ingredientsAllergens[0].Trim().Split(' ').ToList();
                var allergens = ingredientsAllergens[1].Split(", ").Select(x => x.Trim(')')).ToList();

                ingredientList.AddRange(ingredients);

                foreach (var allergen in allergens)
                {
                    if (!allergenMapping.ContainsKey(allergen))
                    {
                        allergenMapping.Add(allergen, new List<IEnumerable<string>>());
                    }
                    allergenMapping[allergen].Add(ingredients);
                }
            }

            return (ingredientList, allergenMapping);
        }

        private static (List<string>, Dictionary<string, string>) CalculateAllergens(Dictionary<string, List<IEnumerable<string>>> allergenMapping)
        {
            List<string> ingredientsWithAllergens = new();
            Dictionary<string, string> ingredientAllergenMap = new();
            List<string> usedUpAllergens = new();
            while (usedUpAllergens.Count < allergenMapping.Count)
            {
                foreach (var allergenMap in allergenMapping.Where(x => !usedUpAllergens.Contains(x.Key)))
                {
                    var ingredientList = allergenMap.Value;

                    var intersectedList = ingredientList.First();
                    for (int i = 1; i < ingredientList.Count; i++)
                    {
                        intersectedList = intersectedList.Intersect(ingredientList[i]);
                    }
                    intersectedList = intersectedList.Except(ingredientsWithAllergens);

                    if (intersectedList.Count() == 1)
                    {
                        var ingredient = intersectedList.First();
                        usedUpAllergens.Add(allergenMap.Key);
                        ingredientsWithAllergens.Add(ingredient);
                        ingredientAllergenMap.Add(allergenMap.Key, ingredient);
                    }

                    for (int i = 0; i < ingredientList.Count; i++)
                    {
                        var ingredient = ingredientList[i];
                        ingredient = ingredient.Where(x => !usedUpAllergens.Contains(x)).ToList();
                    }
                }
            }

            return (ingredientsWithAllergens, ingredientAllergenMap);
        }
    }
}
