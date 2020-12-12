using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day12 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay12.txt");

            var input = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(input.ToList());
            var part2result = SolvePart2(input.ToList());

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<string> input)
        {
            var actions = input.Select(x => Action.Parse(x)).ToList();

            var ferry = new Ferry();
            actions.ForEach(action => ferry.ExecuteActionPartOne(action));

            return ferry.CalculateManhattanDistance().ToString();
        }

        private static string SolvePart2(List<string> input)
        {
            var actions = input.Select(x => Action.Parse(x)).ToList();

            var ferry = new Ferry();
            actions.ForEach(action => ferry.ExecuteActionPartTwo(action));

            return ferry.CalculateManhattanDistance().ToString();
        }

        private class Ferry
        {
            private (int east, int north) FerryPosition { get; set; } = (0, 0);
            private (int east, int north) WaypointPosition { get; set; } = (10, 1);
            private int FerryDirection { get; set; } = 90;

            public void ExecuteActionPartOne(Action action)
            {
                if (action.Operation == 'R' || action.Operation == 'L')
                {
                    TurnFerry(action.Value, action.Operation == 'R');
                }
                else
                {
                    FerryPosition = MoveFerry(action.Operation, action.Value);
                }
            }

            public void ExecuteActionPartTwo(Action action)
            {
                if (action.Operation == 'R' || action.Operation == 'L')
                {
                    RotateWaypoint(action.Value, action.Operation == 'R');
                }
                else if (action.Operation == 'F')
                {
                    MoveFerryTowardsWaypoint(action.Value);
                }
                else
                {
                    WaypointPosition = Move(action.Operation, action.Value, WaypointPosition);
                }
            }

            public int CalculateManhattanDistance()
            {
                return Math.Abs(FerryPosition.east) + Math.Abs(FerryPosition.north);
            }

            #region Part 1
            private (int, int) MoveFerry(char operation, int value)
            {
                return operation switch
                {
                    'F' => Move(DirectionFromDegrees(), value, FerryPosition),
                    _ => Move(operation, value, FerryPosition)
                };
            }

            private void TurnFerry(int moveDegrees, bool clockwise)
            {
                FerryDirection = clockwise
                    ? (FerryDirection + moveDegrees) % 360
                    : (FerryDirection + (360 - moveDegrees)) % 360;
            }

            private char DirectionFromDegrees()
            {
                return FerryDirection switch
                {
                    0 => 'N',
                    90 => 'E',
                    180 => 'S',
                    270 => 'W',
                    _ => throw new Exception("Invalid degree value")
                };
            }
            #endregion

            #region Part 2
            private void MoveFerryTowardsWaypoint(int numberOfTimes)
            {
                (int east, int north) = (WaypointPosition.east * numberOfTimes, WaypointPosition.north * numberOfTimes);
                FerryPosition = (FerryPosition.east + east, FerryPosition.north + north);
            }

            private void RotateWaypoint(int moveDegrees, bool clockwise)
            {
                moveDegrees = clockwise
                    ? 360 - moveDegrees
                    : moveDegrees;

                WaypointPosition = moveDegrees switch
                {
                    0 => WaypointPosition,
                    90 => (-WaypointPosition.north, WaypointPosition.east),
                    180 => (-WaypointPosition.east, -WaypointPosition.north),
                    270 => (WaypointPosition.north, -WaypointPosition.east),
                    _ => throw new Exception("Invalid degree value")
                };
            }
            #endregion

            private static (int east, int north) Move(char operation, int value, (int east, int north) position)
            {
                return operation switch
                {
                    'N' => (position.east, position.north + value),
                    'E' => (position.east + value, position.north),
                    'S' => (position.east, position.north - value),
                    'W' => (position.east - value, position.north),
                    _ => throw new Exception("Invalid operation")
                };
            }
        }

        private record Action(char Operation, int Value)
        {
            public static Action Parse(string rawAction)
            {
                return new Action(rawAction[0], int.Parse(rawAction[1..]));
            }
        }
    }
}
