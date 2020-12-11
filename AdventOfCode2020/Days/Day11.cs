using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day11 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay11.txt");

            var input = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(input.ToList());
            var part2result = SolvePart2(input.ToList());

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<string> input)
        {
            var layout = new Layout(input, checkAdjacentSeatsOnly: true);

            while (layout.ApplyRules(seatLimit: 4)) ;

            return layout.NumberOfOccupiedSeats().ToString();
        }

        private static string SolvePart2(List<string> input)
        {
            var layout = new Layout(input, checkAdjacentSeatsOnly: false);

            while (layout.ApplyRules(seatLimit: 5)) ;

            return layout.NumberOfOccupiedSeats().ToString();
        }

        private class Layout
        {
            private char[,] Area { get; set; }
            private readonly bool checkAdjacentSeatsOnly;
            private readonly int rowLength;
            private readonly int columnLength;

            public Layout(List<string> input, bool checkAdjacentSeatsOnly)
            {
                rowLength = input.Count;
                columnLength = input[0].Length;
                this.checkAdjacentSeatsOnly = checkAdjacentSeatsOnly;

                Area = SetupArea(input);
            }

            /// <summary>
            /// Applies rules for all seats
            /// </summary>
            /// <returns>True if any seats were changed</returns>
            public bool ApplyRules(int seatLimit)
            {
                var seatChanged = false;
                var newArea = new char[rowLength, columnLength];

                for (var row = 0; row < rowLength; row++)
                {
                    for (var column = 0; column < columnLength; column++)
                    {
                        newArea[row, column] = Area[row, column];

                        if (Area[row, column] == 'L' &&
                            NoOccupiedSeatRule(row, column))
                        {
                            newArea[row, column] = '#';
                            seatChanged = true;
                        }
                        else if (Area[row, column] == '#' &&
                            OccupiedSeatsRule(row, column, seatLimit))
                        {
                            newArea[row, column] = 'L';
                            seatChanged = true;
                        }
                    }
                }

                Area = newArea;
                return seatChanged;
            }

            public int NumberOfOccupiedSeats()
            {
                var occupiedSeats = 0;
                foreach (var c in Area)
                {
                    if (c == '#')
                    {
                        occupiedSeats++;
                    }
                }
                return occupiedSeats;
            }

            private char[,] SetupArea(IEnumerable<string> input)
            {
                var area = new char[rowLength, columnLength];

                var row = 0;
                foreach (var line in input)
                {
                    var column = 0;
                    foreach (var c in line)
                    {
                        area[row, column++] = c;
                    }
                    row++;
                }

                return area;
            }

            private bool NoOccupiedSeatRule(int row, int column)
            {
                return GetSeats(row, column).All(s => !s.validSeat || Area[s.row, s.column] != '#');
            }

            private bool OccupiedSeatsRule(int row, int column, int seatLimit)
            {
                var nonOccupiedSeats = GetSeats(row, column).Count(s => !s.validSeat || Area[s.row, s.column] != '#');

                return (8 - nonOccupiedSeats) >= seatLimit;
            }

            private IEnumerable<(int row, int column, bool validSeat)> GetSeats(int row, int column)
            {
                var directions = new (int, int)[] { (0, 1), (1, 0), (0, -1), (-1, 0), (-1, -1), (-1, 1), (1, 1), (1, -1) };
                return directions.Select(direction => SearchSeat(row, column, direction));
            }

            private (int row, int column, bool validSeat) SearchSeat(int row, int column, (int row, int column) direction)
            {
                while (true)
                {
                    row += direction.row;
                    column += direction.column;

                    // If seat is outside the area it's invalid and we can return it
                    if (row >= rowLength || column >= columnLength || row < 0 || column < 0)
                    {
                        return (row, column, false);
                    }

                    if (checkAdjacentSeatsOnly || Area[row, column] != '.')
                    {
                        return (row, column, true);
                    }
                }
            }
        }
    }
}
