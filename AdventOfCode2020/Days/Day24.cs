using AdventOfCode2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public class Day24 : IDay
    {
        public string Solve()
        {
            var inputRaw = EmbeddedResource.ReadInput("InputDay24.txt");

            var lines = inputRaw.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();

            var part1result = SolvePart1(lines.ToList());
            var part2result = SolvePart2(lines.ToList());

            return $"Part 1: {part1result}, Part 2: {part2result}";
        }

        private static string SolvePart1(List<string> input)
        {
            Game game = new();
            game.FlipTiles(input);

            return game.CountBlackTiles().ToString();
        }

        private static string SolvePart2(List<string> input)
        {
            Game game = new();
            game.FlipTiles(input);
            game.PerformArtExhibit(100);

            return game.CountBlackTiles().ToString();
        }

        private class Game
        {
            private Dictionary<Tile, bool> TilesToBlackSideUp { get; } = new();
            private Dictionary<Tile, int> BlackTileCounterMemoization { get; } = new();

            private static readonly (int X, int Y, int Z)[] NeighborCoords =
                new (int, int, int)[6] { (1, -1, 0), (0, -1, 1), (-1, 0, 1), (-1, 1, 0), (0, 1, -1), (1, 0, -1) };

            public Game()
            {
                var referenceTile = new Tile();
                TilesToBlackSideUp.Add(referenceTile, false);
            }

            public int CountBlackTiles()
            {
                return TilesToBlackSideUp.Count(x => x.Value);
            }

            public void PerformArtExhibit(int days)
            {
                for (var i = 0; i < days; i++)
                {
                    ExecuteRules();
                }
            }

            public void FlipTiles(IEnumerable<string> tileMovements)
            {
                foreach (var tileMovement in tileMovements)
                {
                    FlipTile(tileMovement);
                }
            }

            private void FlipTile(string movements)
            {
                var tokens = Lexer.GetTokens(movements);
                Tile tile = new();

                foreach (var direction in tokens)
                {
                    switch (direction)
                    {
                        case Direction.E: tile.X += 1; tile.Y -= 1; break;
                        case Direction.NE: tile.X += 1; tile.Z -= 1; break;
                        case Direction.NW: tile.Y += 1; tile.Z -= 1; break;
                        case Direction.W: tile.Y += 1; tile.X -= 1; break;
                        case Direction.SW: tile.Z += 1; tile.X -= 1; break;
                        case Direction.SE: tile.Z += 1; tile.Y -= 1; break;
                    }
                }

                if (!TilesToBlackSideUp.ContainsKey(tile))
                {
                    TilesToBlackSideUp.Add(tile, false);
                }
                Flip(tile);
            }

            private void ExecuteRules()
            {
                BlackTileCounterMemoization.Clear();

                AddNewAdjacentTiles();

                BlackTileCounterMemoization.Clear();

                FlipAccordingToRules();
            }

            private void AddNewAdjacentTiles()
            {
                List<Tile> newTiles = new();
                foreach (var tile in TilesToBlackSideUp.Keys)
                {
                    AddNewAdjacentTiles(tile, newTiles);
                }
                foreach (var newTile in newTiles)
                {
                    TilesToBlackSideUp.Add(newTile, false);
                }
            }

            private void AddNewAdjacentTiles(Tile tile, List<Tile> newTiles)
            {
                foreach (var (X, Y, Z) in NeighborCoords)
                {
                    var newTile = new Tile { X = tile.X - X, Y = tile.Y - Y, Z = tile.Z - Z };

                    var adjacentBlackTiles = CountBlackAdjacentTiles(newTile);

                    if (!newTiles.Contains(newTile) && !TilesToBlackSideUp.ContainsKey(newTile) && adjacentBlackTiles > 0)
                    {
                        newTiles.Add(newTile);
                        BlackTileCounterMemoization[tile] = adjacentBlackTiles;
                    }
                }
            }

            private void FlipAccordingToRules()
            {
                List<Tile> tilesToFlip = new();

                foreach (var tile in TilesToBlackSideUp.Keys)
                {
                    var blackAdjacentTiles = CountBlackAdjacentTiles(tile);

                    if ((TilesToBlackSideUp[tile] && (blackAdjacentTiles == 0 || blackAdjacentTiles > 2)) ||
                        (!TilesToBlackSideUp[tile] && blackAdjacentTiles == 2))
                    {
                        tilesToFlip.Add(tile);
                    }
                }

                foreach (var tile in tilesToFlip)
                {
                    Flip(tile);
                }
            }

            private int CountBlackAdjacentTiles(Tile tile)
            {
                if (BlackTileCounterMemoization.ContainsKey(tile))
                {
                    return BlackTileCounterMemoization[tile];
                }

                var adjacentBlackTiles = NeighborCoords.Count(neighborCoord =>
                {
                    var neighborTile = new Tile { X = tile.X - neighborCoord.X, Y = tile.Y - neighborCoord.Y, Z = tile.Z - neighborCoord.Z };
                    return TilesToBlackSideUp.ContainsKey(neighborTile) && TilesToBlackSideUp[neighborTile];
                });

                BlackTileCounterMemoization[tile] = adjacentBlackTiles;
                return adjacentBlackTiles;
            }

            private void Flip(Tile tile)
            {
                TilesToBlackSideUp[tile] = !TilesToBlackSideUp[tile];
            }
        }

        private struct Tile : IEquatable<Tile>
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }

            public override int GetHashCode()
            {
                return HashCode.Combine(X, Y, Z);
            }

            public override bool Equals(object obj)
            {
                return obj is Tile tile && Equals(tile);
            }

            public bool Equals(Tile other)
            {
                return X == other.X &&
                       Y == other.Y &&
                       Z == other.Z;
            }
        }

        private static class Lexer
        {
            public static List<Direction> GetTokens(string input)
            {
                List<Direction> directions = new();

                for (var currentIndex = 0; currentIndex < input.Length; currentIndex++)
                {
                    directions.Add(GetToken(input, ref currentIndex));
                }

                return directions;
            }

            private static Direction GetToken(string input, ref int index)
            {
                return (input[index]) switch
                {
                    'n' => input[++index] switch
                    {
                        'e' => Direction.NE,
                        'w' => Direction.NW,
                        _ => throw new Exception(),
                    },
                    's' => input[++index] switch
                    {
                        'e' => Direction.SE,
                        'w' => Direction.SW,
                        _ => throw new Exception(),
                    },
                    'e' => Direction.E,
                    'w' => Direction.W,
                    _ => throw new Exception(),
                };
            }
        }

        private record Token(Direction Direction);

        private enum Direction
        {
            E,
            W,
            NE,
            SE,
            SW,
            NW
        }
    }
}
